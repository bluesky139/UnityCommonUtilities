using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using common;

namespace AirInspector
{
    public enum Error
    {
        OK,
        Error
    }
    public delegate void RecvCallback(Error err, byte[] bytes); // if err != OK, callback will never be called later.

	public class AirMgr : InstanceTemplate<AirMgr>
	{
        enum State
        {
            Start,
            Listening,
            Transmitting,
            Reset
        }
        State state = State.Start;

        AirServer server;
        List<AirCommand> recvCmds = new List<AirCommand>();
        Dictionary<int, AirTweaker> tweakers = new Dictionary<int, AirTweaker>();

        public void RegisterTweaker(AirTweaker tweaker)
        {
			common.Debug.Log("RegisterTweaker " + tweaker.GetType().Name);
            tweakers.Add(tweaker.GetInstanceID(), tweaker);
        }

        public void UnregisterTweaker(AirTweaker tweaker)
        {
            common.Debug.Log("UnregisterTweaker " + tweaker.GetType().Name);
            tweakers.Remove(tweaker.GetInstanceID());
        }

        public void Update()
        {
            switch (state)
            {
            case State.Start:
                server = new AirServer();
                server.StartListening(OnRecv);
                state = State.Listening;
                break;
            case State.Listening:
                if (server.isConnected)
                {
                    common.Debug.Log("state to Transmitting");
                    state = State.Transmitting;
                }
                break;
            case State.Transmitting:
                HandleRecvs();
                break;
            case State.Reset:
                Reset();
                break;
            }
        }

        public void OnDestroy()
        {
            Reset();
        }

        void Reset()
        {
            if (server != null)
                server.Close();
            server = null;
            recvCmds.Clear();
            state = State.Start;
        }

        void HandleRecvs()
        {
            if (recvCmds.Count == 0)
                return;
            AirCommand recvCmd = recvCmds[0];
            recvCmds.RemoveAt(0);
            common.Debug.Log("Handle recv cmd " + recvCmd.op);

            switch (recvCmd.op)
            {
            case AirCommand.OP.GetReq:
                {
                    AirCommand.GetReq req = (AirCommand.GetReq) recvCmd.data;
                    HandleRecvGetReqs(req.item);
                    break;
                }
            case AirCommand.OP.SetReq:
                {
                    AirCommand.SetReq req = (AirCommand.SetReq) recvCmd.data;
                    HandleRecvSetReqs(req.item, req.members);
                    break;
                }
            case AirCommand.OP.GetTweakerListReq:
                {
                    AirCommand.GetTweakerListAck ack = new AirCommand.GetTweakerListAck();
                    ack.headers = new List<AirCommand.TweakerHeader>();
                    foreach (KeyValuePair<int, AirTweaker> kv in tweakers)
                    {
                        AirCommand.TweakerHeader header = new AirCommand.TweakerHeader();
                        header.name = kv.Value.name + " - " + kv.Value.GetType().Name;
                        header.instanceID = kv.Key;
                        header.path = "/" + kv.Value.name;
                        Transform parent = kv.Value.transform.parent;
                        while (parent != null)
                        {
                            header.path = "/" + parent.name + header.path;
                            parent = parent.parent;
                        }
                        ack.headers.Add(header);
                    }

                    AirCommand cmd = new AirCommand(AirCommand.OP.GetTweakerListAck, ack);
                    server.Send(cmd.bytes);
                    break;
                }
            case AirCommand.OP.GetTweakerBodyReq:
                {
                    AirCommand.GetTweakerBodyReq req = (AirCommand.GetTweakerBodyReq) recvCmd.data;
                    AirTweaker tweaker = tweakers[req.header.instanceID];

                    AirCommand.GetTweakerBodyAck ack = new AirCommand.GetTweakerBodyAck();
                    ack.header  = new AirCommand.TweakerHeader();
                    ack.header  = req.header;
                    ack.members = tweaker.Serialize();

                    AirCommand cmd = new AirCommand(AirCommand.OP.GetTweakerBodyAck, ack);
                    server.Send(cmd.bytes);
                    break;
                }
            case AirCommand.OP.SetTweakerBodyReq:
                {
                    AirCommand.SetTweakerBodyReq req = (AirCommand.SetTweakerBodyReq) recvCmd.data;
                    AirTweaker tweaker = tweakers[req.header.instanceID];
                    tweaker.Deserialize(req.members);
                    break;
                }
            /*case AirCommand.OP.GetLogListReq:
                {
                    AirCommand.GetLogListAck ack = new AirCommand.GetLogListAck();
                    DirectoryInfo dirInfo = new DirectoryInfo(common.LogToFile.dir);
                    FileInfo[] filesInfo = dirInfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                    ack.files = new string[filesInfo.Length];
                    for (int i = 0; i < filesInfo.Length; ++i)
                    {
                        ack.files[i] = filesInfo[filesInfo.Length - i - 1].Name;
                    }

                    AirCommand cmd = new AirCommand(AirCommand.OP.GetLogListAck, ack);
                    server.Send(cmd.bytes);
                    break;
                }
            case AirCommand.OP.GetLogFileReq:
                {
                    AirCommand.GetLogFileReq req = (AirCommand.GetLogFileReq) recvCmd.data;
                    AirCommand.GetLogFileAck ack = new AirCommand.GetLogFileAck();
                    Debug.Log("Read log file " + req.file);
                    ack.file = req.file;
                    FileStream stream = new FileStream(common.LogToFile.dir + "/" + req.file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    stream.Close();
                    ack.log = bytes;

                    AirCommand cmd = new AirCommand(AirCommand.OP.GetLogFileAck, ack);
                    server.Send(cmd.bytes);
                    break;
                }*/
            }
        }

        void HandleRecvGetReqs(AirCommand.Item item)
        {
            List<DynamicMember> members = new List<DynamicMember>();

            switch (item)
            {
            case AirCommand.Item.Quality:
                GetQuality(ref members);
                break;
            case AirCommand.Item.QualitySettings:
                GetQualitySettings(ref members);
                break;
            }

            AirCommand.GetAck ack = new AirCommand.GetAck();
            ack.members = members;
            ack.item    = item;
            AirCommand cmd = new AirCommand(AirCommand.OP.GetAck, ack);
            server.Send(cmd.bytes);
        }

        void HandleRecvSetReqs(AirCommand.Item item, List<DynamicMember> members)
        {
            switch (item)
            {
            case AirCommand.Item.Quality:
                SetQuality(ref members);
                break;
            case AirCommand.Item.QualitySettings:
                SetQualitySettings(ref members);
                break;
            }
        }

        void OnRecv(Error err, byte[] bytes)
        {
            if (err != Error.OK)
            {
                common.Debug.LogWarning("Error received.");
                state = State.Reset;
                return;
            }
            AirCommand cmd = new AirCommand(bytes);
            recvCmds.Add(cmd);
        }

        void GetQuality(ref List<DynamicMember> members)
        {
            DynamicMember member = new DynamicMember();
            member.name  = "quality";
            member.type  = DeviceProfiles.quality.GetType().AssemblyQualifiedName;
            member.value = DeviceProfiles.quality;
            members.Add(member);
        }

        void SetQuality(ref List<DynamicMember> members)
        {
            DynamicMember member = members[0];
            DeviceProfiles.quality = (DeviceProfiles.Quality) member.value;
        }

        void GetQualitySettings(ref List<DynamicMember> members)
        {
            string[] commons = DeviceProfiles.GetCommonList();
            foreach (string common in commons)
            {
                object value = DeviceProfiles.GetCommonValue(common);

                DynamicMember member = new DynamicMember();
                member.name  = common.Replace('.', '_');
                member.type  = value.GetType().AssemblyQualifiedName;
                member.value = value;
                members.Add(member);
            }

            PropertyInfo[] properties = typeof(QualitySettings).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetGetMethod() == null || !property.GetGetMethod().IsStatic || property.GetSetMethod() == null)
                    continue;
                object value = property.GetValue(null, null);
                if (!AirUtils.TestSerializable(value))
                    continue;
                if (property.Name == "names" || property.Name == "currentLevel")
                    continue;

				common.Debug.Log("Property " + property.Name + " " + value);
                DynamicMember member = new DynamicMember();
                member.name  = property.Name;
                member.type  = property.PropertyType.AssemblyQualifiedName;
                member.value = value;
                members.Add(member);
            }
        }

        void SetQualitySettings(ref List<DynamicMember> members)
        {
            string[] commons = DeviceProfiles.GetCommonList();
            foreach (string common in commons)
            {
                for (int i = 0; i < members.Count; ++i)
                {
                    DynamicMember member = members[i];
                    if (member.name == common.Replace('.', '_'))
                    {
                        DeviceProfiles.SetCommonValue(common, member.value);
                        members.RemoveAt(i);
                        break;
                    }
                }
            }

            Type type = typeof(QualitySettings);
            foreach (DynamicMember member in members)
            {
                PropertyInfo prop = type.GetProperty(member.name);
                prop.SetValue(null, member.value, null);
            }
        }
    }
}
