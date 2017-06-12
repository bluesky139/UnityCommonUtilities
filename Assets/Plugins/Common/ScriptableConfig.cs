using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace common
{ 
    public abstract class ScriptableConfig<T> : ScriptableObject where T : ScriptableObject
    {
        private static string SettingsAssetName = typeof(T).Name;
        protected string SettingsRootPath
        {
            get
            {
                return "Assets/Resources/";
            }
        }

        private const  string SettingsFolder   = "ScriptableConfig/";
        private const  string SettingsAssetExtension = ".asset";

        private static T instance_;
        public  static T instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = Resources.Load<T>(SettingsFolder + SettingsAssetName);
                #if UNITY_EDITOR
                    if (instance_ == null)
                    {
                        instance_ = ScriptableObject.CreateInstance<T>();
                        string folder = ((ScriptableConfig<T>) (object) instance_).SettingsRootPath + SettingsFolder;
                        string file = folder + SettingsAssetName + SettingsAssetExtension;
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);
                        AssetDatabase.CreateAsset(instance_, file);
                        EditorApplication.SaveAssets();
                    }
                #endif
                    Debug.Assert(instance_ != null, typeof(T).Name + " is null.");
                    ((ScriptableConfig<T>) (object) instance_).Initialize();
                }
                return instance_;
            }
        }

        public virtual void Initialize()
        {
        }
    }
}