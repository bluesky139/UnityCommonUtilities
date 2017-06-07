using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace AirInspector
{
    public class AirCommand
    {
        public enum OP
        {
            GetRootObjects = 0,
            GetObjectDetails,
            GetReq,
            GetAck,
            SetReq,
            GetTweakerListReq,
            GetTweakerListAck,
            GetTweakerBodyReq,
            GetTweakerBodyAck,
            SetTweakerBodyReq,
            GetLogListReq,
            GetLogListAck,
            GetLogFileReq,
            GetLogFileAck,
            _Unknown
        }

        public enum Item
        {
            Quality,
            QualitySettings,
            _Count
        }

        public class TweakerHeader
        {
            public string name;
            public int instanceID;
            public string path;

            public override bool Equals(object obj)
            {
                if (!(obj is TweakerHeader))
                    return false;
                return instanceID == ((TweakerHeader) obj).instanceID;
            }

            public bool EqualAll(TweakerHeader obj)
            {
                return instanceID == obj.instanceID && name == obj.name && path == obj.path;
            }

            public override int GetHashCode()
            {
                return instanceID;
            }
        }

        public struct GetRootObjectsAck
        {
            public List<string> objs;
        }

        public struct GetObjectDetails
        {
            public string path;
        }

        public struct GetObjectDetailsAck
        {
            public string xml;
        }

        public struct GetReq
        {
            public Item item;
        }

        public struct GetAck
        {
            public Item item;
            public List<DynamicMember> members;
        }

        public struct SetReq
        {
            public Item item;
            public List<DynamicMember> members;
        }

        public struct GetTweakerListReq
        {
        }

        public struct GetTweakerListAck
        {
            public List<TweakerHeader> headers;
        }

        public struct GetTweakerBodyReq
        {
            public TweakerHeader header;
        }

        public struct GetTweakerBodyAck
        {
            public TweakerHeader header;
            public List<DynamicMember> members;
        }

        public struct SetTweakerBodyReq
        {
            public TweakerHeader header;
            public List<DynamicMember> members;
        }

        public struct GetLogListReq
        {
        }

        public struct GetLogListAck
        {
            public string[] files;
        }

        public struct GetLogFileReq
        {
            public string file;
        }

        public struct GetLogFileAck
        {
            public string file;
            public byte[] log;
        }

        public OP op { get; private set; }
        public object data { get; private set; }
        public byte[] bytes
        {
            get
            {
                return EncodeCommand(op, data);
            }
        }

        public AirCommand(OP op, object req)
        {
            this.op = op;
            this.data = req;
        }

        public AirCommand(byte[] bytes)
        {
            OP op;
            object data;
            DecodeCommand(bytes, out op, out data);
            this.op = op;
            this.data = data;
        }

        byte[] EncodeCommand(OP op, object obj)
        {
            byte[] opBytes = BitConverter.GetBytes((int)op);

            List<DynamicMember> members = null;
            List<string> extraTypes = new List<string>();
            if (obj is GetAck || obj is SetReq || obj is GetTweakerBodyAck || obj is SetTweakerBodyReq)
            {
                if (obj is GetAck)
                    members = ((GetAck) obj).members;
                else if (obj is SetReq)
                    members = ((SetReq) obj).members;
                else if (obj is GetTweakerBodyAck)
                    members = ((GetTweakerBodyAck) obj).members;
                else if (obj is SetTweakerBodyReq)
                    members = ((SetTweakerBodyReq) obj).members;
            }

            Type[] types = null;
            byte[] typesBytes = AirUtils.SerializeExtraTypes(members, out types);
            byte[] typesLenBytes = BitConverter.GetBytes(typesBytes.Length);

            byte[] objBytes = AirUtils.SerializeObj(obj, types);
            byte[] objLenBytes = BitConverter.GetBytes(objBytes.Length);

            MemoryStream stream = new MemoryStream();
            stream.Seek(sizeof(int), SeekOrigin.Begin);
            stream.Write(opBytes,       0, opBytes.Length);
            stream.Write(typesLenBytes, 0, typesLenBytes.Length);
            stream.Write(typesBytes,    0, typesBytes.Length);
            stream.Write(objLenBytes,   0, objLenBytes.Length);
            stream.Write(objBytes,      0, objBytes.Length);

            byte[] lenBytes = BitConverter.GetBytes((int)(stream.Length - sizeof(int)));
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(lenBytes, 0, lenBytes.Length);
            return stream.ToArray();
        }

        void DecodeCommand(byte[] bytes, out OP op, out object obj)
        {
            MemoryStream bytesStream = new MemoryStream(bytes);
            byte[] lenBytes = new byte[sizeof(int)];
            bytesStream.Read(lenBytes, 0, sizeof(int));
            int len = BitConverter.ToInt32(lenBytes, 0);
            if (len == bytes.Length - lenBytes.Length)
            {
                byte[] opBytes = new byte[sizeof(int)];
                bytesStream.Read(opBytes, 0, opBytes.Length);
                op = (OP)BitConverter.ToInt32(opBytes, 0);

                byte[] typesLenBytes = new byte[sizeof(int)];
                bytesStream.Read(typesLenBytes, 0, typesLenBytes.Length);
                int typesLen = BitConverter.ToInt32(typesLenBytes, 0);
                byte[] typesBytes = new byte[typesLen];
                bytesStream.Read(typesBytes, 0, typesBytes.Length);
                Type[] extraTypes = AirUtils.DeserializeExtraTypes(typesBytes);

                byte[] objLenBytes = new byte[sizeof(int)];
                bytesStream.Read(objLenBytes, 0, objLenBytes.Length);
                int objLen = BitConverter.ToInt32(objLenBytes, 0);
                byte[] objBytes = new byte[objLen];
                bytesStream.Read(objBytes, 0, objBytes.Length);
                
                Type type = Type.GetType("AirInspector.AirCommand+" + op);
                obj = AirUtils.DeserializeObj(type, objBytes, extraTypes);
            }
            else
            {
                Debug.LogError("Decode command error, len not match.");
                op = OP._Unknown;
                obj = null;
            }
        }
    }
}
