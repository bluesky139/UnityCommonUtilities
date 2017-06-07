using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using common;

namespace AirInspector
{
    public abstract class AirTweaker : MonoBehaviour
    {
        protected abstract Dictionary<string, object> GetSerializeObjs();
        protected abstract void SetFromSerializedObjs(Dictionary<string, object> objs);

        [Serializable]
        public class Data
        {
            public byte[] extraTypes;
            public byte[] members;
        }
        public Data[] data;

        protected virtual void Awake()
        {
            if (this.data != null && this.data.Length != 0)
            {
                Data data = this.data[(int) DeviceProfiles.quality];
                if (data != null && data.members != null && data.members.Length != 0)
                {
                    Type[] types = AirUtils.DeserializeExtraTypes(data.extraTypes);
                    List<DynamicMember> members = (List<DynamicMember>) AirUtils.DeserializeObj(typeof(List<DynamicMember>), data.members, types);
                    Deserialize(members);
                }
                else
                {
					common.Debug.LogWarning("Data of quality " + DeviceProfiles.quality + " in " + name + " is missing, nothing will be changed.");
                }
            }

            AirMgr.instance.RegisterTweaker(this);
        }

        void OnDestroy()
        {
            AirMgr.instance.UnregisterTweaker(this);
        }

        public List<DynamicMember> Serialize()
        {
            Dictionary<string, object> objs = GetSerializeObjs();
            List<DynamicMember> members = new List<DynamicMember>();
            foreach (KeyValuePair<string, object> kv in objs)
            {
                if (!AirUtils.TestSerializable(kv.Value))
                {
					common.Debug.LogError("Can't serialize " + kv.Key + " in " + GetType().Name);
                    continue;
                }

                DynamicMember member = new DynamicMember();
                member.name  = kv.Key;
                member.type  = kv.Value.GetType().AssemblyQualifiedName;
                member.value = kv.Value;
                members.Add(member);
            }
            return members;
        }

        public void Deserialize(List<DynamicMember> members)
        {
            Dictionary<string, object> objs = new Dictionary<string, object>();
            foreach (DynamicMember member in members)
            {
                objs.Add(member.name, member.value);
            }
            SetFromSerializedObjs(objs);
        }
    }
}
