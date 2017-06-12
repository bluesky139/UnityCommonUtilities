#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace common
{ 
    public class DevMacro : ScriptableConfig<DevMacro>
    {
        [Serializable]
        public class Macro
        {
            public string name;
            public string type;
            public string desc;

            [SerializeField]
            protected bool enabled_ = true;
            public virtual bool enabled
            {
                get
                {
                    return enabled_;
                }
                set
                {
                    if (enabled != value)
                    {
                        enabled_ = value;
                        lastModifyTime = DateTime.Now;
                    }
                }
            }

            [SerializeField]
            protected string lastModifyTime_;
            public virtual DateTime lastModifyTime
            {
                get
                {
                    return DateTime.ParseExact(lastModifyTime_, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                set
                {
                    lastModifyTime_ = value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }

            public Macro(string name, string type)
            {
                this.name = name;
                this.type = type;
                enabled = false;
            }

            public Macro Clone()
            {
                Macro macro = new Macro(name, type);
                macro.desc = desc;
                macro.enabled_ = enabled_;
                macro.lastModifyTime_ = lastModifyTime_;
                return macro;
            }
        }

        [SerializeField]
        [HideInInspector]
        protected List<Macro> macroList;
        public Dictionary<string /*type*/, List<Macro>> macros
        {
            get
            {
                Dictionary<string, List<Macro>> macros_ = new Dictionary<string, List<Macro>>();
                if (macroList != null)
                {
                    foreach (Macro macro in macroList)
                    {
                        if (!macros_.ContainsKey(macro.type))
                            macros_.Add(macro.type, new List<Macro>());
                        macros_[macro.type].Add(macro.Clone());
                    }
                }
                return macros_;
            }
            set
            {
                macroList = new List<Macro>();
                foreach (KeyValuePair<string, List<Macro>> kv in value)
                {
                    foreach (Macro macro in kv.Value)
                    {
                        macroList.Add(macro.Clone());
                    }
                }
            }
        }

        public Macro GetMacro(string name, string type)
        {
            if (!macros.ContainsKey(type))
                return null;
            foreach (Macro macro in macros[type])
            {
                if (macro.name == name)
                    return macro;
            }
            return null;
        }
    }
}
#endif