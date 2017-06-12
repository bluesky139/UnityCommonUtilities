using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using System.Text.RegularExpressions;
using Macro = common.DevDebug.Macro;
using UnityEditor.SceneManagement;

namespace common
{ 
    [InitializeOnLoad]
    [CustomEditor(typeof(DevDebug))]
    public class DevDebugEditor : BaseMacroEditor
    {
        static string recvLogPath = "";
        Dictionary<string, List<Macro>> macros;

        [MenuItem("Common/Macro/Dev Debug", priority = 50)]
        public static void Edit()
        {
            Selection.activeObject = DevDebug.instance;
        }

        void OnEnable()
        {
            macros = DevDebug.instance.macros;
        }

        public override void OnInspectorGUI()
        {
            if (OnRefreshing())
                return;

            EditorGUILayout.Separator();
            if (GUILayout.Button("Edit Macro"))
            {
                Selection.activeObject = DevMacro.instance;
            }
            if (GUILayout.Button("Refresh rsp"))
            {
                RefreshRsp();
            }
            if (GUILayout.Button("Recompile Script"))
            {
                RecompileScript();
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Generate DevLog_Gen.cs"))
            {
                GenerateDevLog_Gen();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Note: Remember to press apply after toggling.", EditorStyles.boldLabel);
        
            foreach (KeyValuePair<string, List<Macro>> kv in macros)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField(kv.Key + ":");
                foreach (Macro macro in kv.Value)
                {
				    if (macro.valid)
				    {
					    if (!string.IsNullOrEmpty(macro.original.desc))
						    EditorGUILayout.LabelField(macro.original.desc);
					    macro.enabled = EditorGUILayout.Toggle(macro.name, macro.enabled);
				    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Revert"))
            {
                macros = DevDebug.instance.macros;
                Repaint();
            }
            if (GUILayout.Button("Apply"))
            {
                DevDebug.instance.macros = macros;
                EditorUtility.SetDirty(DevDebug.instance);
                EditorApplication.SaveAssets();
                RefreshRsp();
            }
            EditorGUILayout.EndHorizontal();
	    }

        private void GenerateDevLog_Gen()
        {
            List<string> logs = new List<string>();
            foreach (DevMacro.Macro macro in DevMacro.instance.macros["Logs"])
            {
                if (macro.name != "LOG_TO_FILE")
                    logs.Add(macro.name);
            }
            string content = "// Generated from DevLog_Template by DevDebugEditor.\n";
            content += "// Add new tag in DevDebug.GetLogList(), then press button CustomBuild -> DevDebug -> Generate DevLog_Gen.cs\n";
            content += "//\n\n";
            foreach (string log in logs)
            {
                string template = File.ReadAllText(EditorEnv.dstUnityProjectPlugins + "/Macro/DevLog_Template.cs");
                template = template.Replace("_NAMESPACE_", log.Substring(0, log.Length - 4).ToLower());
                template = template.Replace("_TAG_", log.Substring(0, log.Length - 4));
                template = template.Replace("_MACRO_", log);
                content += template;
            }
			var path = EditorEnv.dstUnityProjectPlugins + "/Macro/DevLog_Gen.cs";
			File.WriteAllText(path, content);
        }

        static DevDebugEditor()
        {
            RefreshRsp();
        }
    }
}