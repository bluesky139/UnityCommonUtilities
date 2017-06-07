using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AirInspector
{
    class SerializedScript
    {
        public List<DynamicMember>      members;
        public Component                memberWithMono;
        public SerializedObject         serializedObj;
        public List<SerializedProperty> props;

        public void Clear()
        {
            if (memberWithMono != null)
                MonoBehaviour.DestroyImmediate(memberWithMono.gameObject);
        }
    }

    class AirEditorUtils
    {
        public static SerializedScript CreateSerializeScript(List<DynamicMember> members, string name)
        {
            Type type = AirUtils.CompileResultType(members, name, typeof(MonoBehaviour));
            GameObject obj = new GameObject("Air_" + name);
//            MonoBehaviour.DontDestroyOnLoad(obj); // DontDestroyOnLoad cannot be called in Editor (5.3.5p1)
            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            Component settings = obj.AddComponent(type);
            Type settingsType = settings.GetType();
            foreach (DynamicMember member in members)
            {
                FieldInfo field = settingsType.GetField(member.name);
                field.SetValue(settings, member.value);
            }

            SerializedObject s = new SerializedObject(settings);
            List<SerializedProperty> props = new List<SerializedProperty>();
            foreach (DynamicMember member in members)
            {
                SerializedProperty prop = s.FindProperty(member.name);
                props.Add(prop);
            }

            SerializedScript script = new SerializedScript();
            script.members        = members;
            script.memberWithMono = settings;
            script.serializedObj  = s;
            script.props          = props;
            return script;
        }
    }
}
