using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Macro = common.DevMacro.Macro;

namespace common
{ 
    [CustomEditor(typeof(DevMacro))]
    class DevMacroEditor : BaseMacroEditor
    {
        Vector2 scrollPos = Vector2.zero;
        string newMacro = "";
        string newType  = "";
        Dictionary<string, List<Macro>> macros;

        void OnEnable()
        {
            macros = DevMacro.instance.macros;
        }

        public override void OnInspectorGUI()
        {
            if (OnRefreshing())
                return;

            if (GUILayout.Button("Back"))
            {
                Selection.activeObject = DevDebug.instance;
            }
            EditorGUILayout.Separator();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (KeyValuePair<string, List<Macro>> kv in macros)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(kv.Key + ":");
                newMacro = EditorGUILayout.TextField(newMacro);
                if (GUILayout.Button("Add Macro"))
                {
                    if (string.IsNullOrEmpty(newMacro))
                        return;
                    kv.Value.Add(new Macro(newMacro.ToUpper(), kv.Key)); 
                    Repaint();
                }
                EditorGUILayout.EndHorizontal();
                foreach (Macro macro in kv.Value)
                {
                    EditorGUILayout.Separator();
                    macro.name = EditorGUILayout.TextField("  Name", macro.name);
                    macro.desc = EditorGUILayout.TextField("  Description", macro.desc);
                    macro.enabled = EditorGUILayout.Toggle("  Enable By Default", macro.enabled);
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            newType = EditorGUILayout.TextField(newType);
            if (GUILayout.Button("Add Type"))
            {
                if (string.IsNullOrEmpty(newType))
                    return;
                macros.Add(newType, new List<Macro>());
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Revert"))
            {
                macros = DevMacro.instance.macros;
                Repaint();
            }
            if (GUILayout.Button("Apply"))
            {
                DevMacro.instance.macros = macros;
                EditorUtility.SetDirty(DevMacro.instance);
                EditorApplication.SaveAssets();
                RefreshRsp();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}