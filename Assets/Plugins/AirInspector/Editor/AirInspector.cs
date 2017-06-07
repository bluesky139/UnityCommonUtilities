using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using common;

namespace AirInspector
{
    class AirInspector : EditorWindow
    {
        [MenuItem("Common/Air Inspector", priority = 110)]
        public static void Edit()
        {
            EditorWindow.GetWindow(typeof(AirInspector));
        }

        string ip = "127.0.0.1";
        int port = 9999;
        AirClient client;
        List<AirCommand> recvCmds = new List<AirCommand>();
        bool reset = false;

        DeviceProfiles.Quality quality = DeviceProfiles.Quality._Overflow;
        SerializedScript[] items = new SerializedScript[(int) AirCommand.Item._Count];
        Dictionary<AirCommand.TweakerHeader, SerializedScript> tweakers = new Dictionary<AirCommand.TweakerHeader, SerializedScript>();
        string[] logFiles;

        void OnEnable()
        {
        }

        void OnDisable()
        {
            Reset();
        }

		Vector2 scrollPosition = Vector2.zero;
		void OnGUI()
        {
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			EditorGUILayout.BeginHorizontal();
            ip   = EditorGUILayout.TextField(ip);
            port = int.Parse(EditorGUILayout.TextField("" + port));
            if (GUILayout.Button("Connect"))
            {
                client = new AirClient();
                client.StartConnect(ip, port, OnRecv);
            }
            EditorGUILayout.EndHorizontal();

            if (client != null && client.isConnected)
            {
                OnGUIItem();
                OnGUITweakers();
            }
			EditorGUILayout.EndScrollView();
		}

		void OnGUIItem()
        {
            for (int i = 0; i < (int) AirCommand.Item._Count; ++i)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[" + (AirCommand.Item) i + "]", EditorStyles.boldLabel);
                if (GUILayout.Button("Get"))
                {
                    AirCommand.GetReq req = new AirCommand.GetReq();
                    req.item = (AirCommand.Item) i;
                    AirCommand cmd = new AirCommand(AirCommand.OP.GetReq, req);
                    client.Send(cmd.bytes);
                }

                if (GUILayout.Button("Set"))
                {
                    items[i].serializedObj.ApplyModifiedProperties();
                    Component component = items[i].memberWithMono;
                    List<DynamicMember> members = items[i].members;
                    for (int j = 0; j < members.Count; ++j)
                    {
                        DynamicMember member = members[j];
                        FieldInfo field = component.GetType().GetField(member.name);
                        member.value = field.GetValue(component);
                        members[j] = member;
                    }

                    AirCommand.SetReq req = new AirCommand.SetReq();
                    req.item    = (AirCommand.Item) i;
                    req.members = members;
                    AirCommand cmd = new AirCommand(AirCommand.OP.SetReq, req);
                    client.Send(cmd.bytes);

                    if (req.item == AirCommand.Item.Quality)
                    {
                        quality = (DeviceProfiles.Quality) members[0].value;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (items[i] == null || items[i].props == null)
                    continue;
                List<SerializedProperty> props = items[i].props;
                foreach (SerializedProperty prop in props)
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
        }

        void OnGUITweakers()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("Get Tweakers"))
            {
                AirCommand.GetTweakerListReq req = new AirCommand.GetTweakerListReq();
                AirCommand cmd = new AirCommand(AirCommand.OP.GetTweakerListReq, req);
                client.Send(cmd.bytes);
            }

            foreach (KeyValuePair<AirCommand.TweakerHeader, SerializedScript> kv in tweakers)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[" + kv.Key.name + "]", EditorStyles.boldLabel);
                if (GUILayout.Button("Get"))
                {
                    AirCommand.GetTweakerBodyReq req = new AirCommand.GetTweakerBodyReq();
                    req.header = kv.Key;
                    AirCommand cmd = new AirCommand(AirCommand.OP.GetTweakerBodyReq, req);
                    client.Send(cmd.bytes);
                }

                if (GUILayout.Button("Set"))
                {
                    kv.Value.serializedObj.ApplyModifiedProperties();
                    Component component = kv.Value.memberWithMono;
                    List<DynamicMember> members = kv.Value.members;
                    for (int j = 0; j < members.Count; ++j)
                    {
                        DynamicMember member = members[j];
                        FieldInfo field = component.GetType().GetField(member.name);
                        member.value = field.GetValue(component);
                        members[j] = member;
                    }

                    AirCommand.SetTweakerBodyReq req = new AirCommand.SetTweakerBodyReq();
                    req.header  = kv.Key;
                    req.members = members;
                    AirCommand cmd = new AirCommand(AirCommand.OP.SetTweakerBodyReq, req);
                    client.Send(cmd.bytes);
                }
                EditorGUILayout.EndHorizontal();

                if (kv.Value != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Find in hierarchy"))
                    {
                        GameObject obj = GameObject.Find(kv.Key.path);
                        if (obj == null)
                            return;
                        Selection.activeGameObject = obj;
                        //UnityEngine.Object prefabObj = PrefabUtility.GetPrefabParent(obj);
                    }
                    if (GUILayout.Button("Save to select GameObject"))
                    {
                        GameObject obj = Selection.activeGameObject;
                        if (obj == null)
                            return;
                        AirTweaker tweaker = obj.GetComponent<AirTweaker>();
                        if (tweaker == null)
                            return;
                        if (tweaker.data == null || tweaker.data.Length == 0)
                            tweaker.data = new AirTweaker.Data[(int) DeviceProfiles.Quality._Overflow];

                        List<string> extraTypes = new List<string>();
                        foreach (DynamicMember member in kv.Value.members)
                        {
                            if (!extraTypes.Contains(member.type))
                                extraTypes.Add(member.type);
                        }
                        AirTweaker.Data data = new AirTweaker.Data();
                        Type[] types;
                        data.extraTypes = AirUtils.SerializeExtraTypes(kv.Value.members, out types);
                        data.members = AirUtils.SerializeObj(kv.Value.members, types);
                        tweaker.data[(int) quality] = data;

						common.Debug.Log("Saved air data to " + obj.name + " of " + quality);
                        AirTweakerEditor.current.FocusOnQuality(quality);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.LabelField("Path", kv.Key.path);
                EditorGUILayout.LabelField("InstanceID", "" + kv.Key.instanceID);

                if (kv.Value == null || kv.Value.props == null)
                    continue;
                foreach (SerializedProperty prop in kv.Value.props)
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }

            EditorGUILayout.Separator();
            /*if (GUILayout.Button("Get Log List"))
            {
                AirCommand.GetLogListReq req = new AirCommand.GetLogListReq();
                AirCommand cmd = new AirCommand(AirCommand.OP.GetLogListReq, req);
                client.Send(cmd.bytes);
            }*/

            if (logFiles != null)
            {
                foreach (string logFile in logFiles)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(logFile);
                    if (GUILayout.Button("Get"))
                    {
                        AirCommand.GetLogFileReq req = new AirCommand.GetLogFileReq();
                        req.file = logFile;
                        AirCommand cmd = new AirCommand(AirCommand.OP.GetLogFileReq, req);
                        client.Send(cmd.bytes);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        void OnRecv(Error err, byte[] bytes)
        {
            if (err == Error.OK)
            {
                AirCommand cmd = new AirCommand(bytes);
                recvCmds.Add(cmd);
            }
            else
            {
                reset = true;
            }
        }

        void Reset()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
            recvCmds.Clear();

            for (int i = 0; i < (int) AirCommand.Item._Count; ++i)
            {
                if (items[i] != null)
                    items[i].Clear();
                items[i] = null;
            }
            foreach (KeyValuePair<AirCommand.TweakerHeader, SerializedScript> kv in tweakers)
            {
                if (kv.Value != null)
                    kv.Value.Clear();
            }
            tweakers.Clear();

            logFiles = null;
            reset = false;
            Repaint();
        }

        void Update()
        {
            if (reset)
            {
                Reset();
                return;
            }

            if (recvCmds.Count == 0)
                return;
            AirCommand cmd = recvCmds[0];
            recvCmds.RemoveAt(0);

			common.Debug.Log("Handle recv cmd " + cmd.op);
            HandleRecvCmd(cmd);
        }

        void HandleRecvCmd(AirCommand cmd)
        {
            switch (cmd.op)
            {
            case AirCommand.OP.GetAck:
                {
                    AirCommand.GetAck ack = (AirCommand.GetAck) cmd.data;
                    SerializedScript script = AirEditorUtils.CreateSerializeScript(ack.members, ack.item.ToString());
                    items[(int) ack.item] = script;

                    if (ack.item == AirCommand.Item.Quality)
                    {
                        quality = (DeviceProfiles.Quality) ack.members[0].value;
                    }
                    break;
                }
            case AirCommand.OP.GetTweakerListAck:
                {
                    AirCommand.GetTweakerListAck ack = (AirCommand.GetTweakerListAck) cmd.data;
                    tweakers.Clear();
                    foreach (AirCommand.TweakerHeader header in ack.headers)
                    {
                        tweakers.Add(header, null);
                    }
                    break;
                }
            case AirCommand.OP.GetTweakerBodyAck:
                {
                    AirCommand.GetTweakerBodyAck ack = (AirCommand.GetTweakerBodyAck) cmd.data;
                    SerializedScript script = AirEditorUtils.CreateSerializeScript(ack.members, ack.header.name);
                    tweakers[ack.header] = script;
                    break;
                }
            case AirCommand.OP.GetLogListAck:
                {
                    AirCommand.GetLogListAck ack = (AirCommand.GetLogListAck) cmd.data;
                    logFiles = ack.files;
                    break;
                }
            case AirCommand.OP.GetLogFileAck:
                {
                    AirCommand.GetLogFileAck ack = (AirCommand.GetLogFileAck) cmd.data;
                    string dir = "./log_dev_mobile";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    string file = dir + "/" + ack.file;
                    if (File.Exists(file))
                        File.Delete(file);
                    File.WriteAllBytes(file, ack.log);
					common.Debug.Log("Mobile log is saved to " + file);
                    break;
                }
            }
            Repaint(); 
        }
    }

}

