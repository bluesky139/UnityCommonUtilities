using UnityEditor;
using common;
using System.Collections.Generic;
using System.IO;

class LightningRebuildEditorTest
{
	[MenuItem("Test/LightningRebuild/Build Android")]
	static void Build()
	{
		LightningRebuild.PreBuild(BuildTarget.Android);
		if (!Directory.Exists("./TestOutput/LightningRebuild"))
			Directory.CreateDirectory("./TestOutput/LightningRebuild");
		BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), "./TestOutput/LightningRebuild/test.apk", BuildTarget.Android, BuildOptions.None);
	}

	static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }
}