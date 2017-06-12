#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEditor;

namespace common
{ 
    public class DevDebug : ScriptableConfig<DevDebug>
    {
        [Serializable]
        public class Macro : DevMacro.Macro
        {
            public DevMacro.Macro original
            {
                get
                {
                    return DevMacro.instance.GetMacro(name, type);
                }
            }

		    public bool valid
		    {
			    get
			    {
				    return original != null;
			    }
		    }

		    public override bool enabled
            {
                get
                {
				    if (valid) // after branch switch, macro diffs, original is null
					    return lastModifyTime > original.lastModifyTime ? enabled_ : original.enabled;
				    return false;
			    }
                set
                {
                    base.enabled = value; 
                }
            }

            public override DateTime lastModifyTime
            {
                get
                {
                    if (string.IsNullOrEmpty(lastModifyTime_))
                        return original.lastModifyTime;
                    return base.lastModifyTime;
                }
                set
                {
                    base.lastModifyTime = value;
                }
            }

            public Macro(string name, string type)
                : base(name, type)
            {
                lastModifyTime = original.lastModifyTime;
            }

            public new Macro Clone()
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
        List<Macro> macroList;
        public Dictionary<string /*type*/, List<Macro>> macros
        {
            get
            {
                Dictionary<string, List<Macro>> macros_ = new Dictionary<string, List<Macro>>();
                if (macroList != null)
                {
                    foreach (Macro macro in macroList)
                    {
                        if (macro.original == null)
                        {
                            Debug.Log("Macro " + macro.name + " is removed from original macro list.");
                            continue;
                        }

                        if (!macros_.ContainsKey(macro.type))
                            macros_.Add(macro.type, new List<Macro>());
                        macros_[macro.type].Add(macro.Clone());
                    }
                }

                foreach (KeyValuePair<string, List<DevMacro.Macro>> kv in DevMacro.instance.macros)
                {
                    if (!macros_.ContainsKey(kv.Key))
                        macros_.Add(kv.Key, new List<Macro>());
                    List<Macro> myMacros = macros_[kv.Key];
                    foreach (DevMacro.Macro macro in kv.Value)
                    {
                        bool hasMacro = false;
                        foreach (Macro myMacro in myMacros)
                        {
                            if (macro.name == myMacro.name)
                            {
                                hasMacro = true;
                                break;
                            }
                        }
                        if (!hasMacro)
                            myMacros.Add(new Macro(macro.name, macro.type));
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
                        macroList.Add(macro); 
                    }
                }
            }
        }
    }
}
#endif