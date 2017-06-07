using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

using common;

namespace AirInspector
{
    [CustomEditor(typeof(AirTweaker), true)]
    class AirTweakerEditor : Editor
    {
        DeviceProfiles.Quality quality = DeviceProfiles.Quality.Fastest;
        SerializedScript script;

        private static AirTweakerEditor current_;
        public  static AirTweakerEditor current
        {
            get
            {
                return current_;
            }
        }

        public void FocusOnQuality(DeviceProfiles.Quality quality)
        {
            this.quality = quality;
            Repaint();
        }

        void OnEnable()
        {
            current_ = this;
        }

        void OnDisable()
        {
            current_ = null;
            if (script != null)
                script.Clear();
        }

        public override void OnInspectorGUI()
        {
            AirTweaker tweaker = (AirTweaker) target;
            if (tweaker.data == null || tweaker.data.Length == 0)
            {
                EditorGUILayout.TextField("No data");
                return;
            }

            AirTweaker.Data data = null;
            List<string> missingData = new List<string>();
            for (int i = 0; i < (int) DeviceProfiles.Quality._Overflow; ++i)
            {
                data = tweaker.data[i];
                if (data == null || data.members == null || data.members.Length == 0)
                    missingData.Add(((DeviceProfiles.Quality) i).ToString());
            }
            if (missingData.Count > 0)
            {
                EditorGUILayout.HelpBox("Missing data of quality " + string.Join(",", missingData.ToArray()), MessageType.Warning);
            }

            DeviceProfiles.Quality q = (DeviceProfiles.Quality) EditorGUILayout.EnumPopup("Quality", quality);
            if (q == DeviceProfiles.Quality._Overflow)
                return;
            quality = q;

            EditorGUILayout.Separator();
            data = tweaker.data[(int) quality];
            if (data == null || data.members == null || data.members.Length == 0)
            {
                EditorGUILayout.LabelField("No data");
                return;
            }

            if (script != null)
                script.Clear();
            Type[] types = AirUtils.DeserializeExtraTypes(data.extraTypes);
            List<DynamicMember> members = (List<DynamicMember>) AirUtils.DeserializeObj(typeof(List<DynamicMember>), data.members, types);
            script = AirEditorUtils.CreateSerializeScript(members, tweaker.name);
            foreach (SerializedProperty prop in script.props)
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            if (GUI.changed)
            {
                script.serializedObj.ApplyModifiedProperties();
                Component component = script.memberWithMono;
                members = script.members;
                for (int j = 0; j < members.Count; ++j)
                {
                    DynamicMember member = members[j];
                    FieldInfo field = component.GetType().GetField(member.name);
                    member.value = field.GetValue(component);
                    members[j] = member;
                }
                data.members = AirUtils.SerializeObj(members, types);
            }
        }
    }
}
