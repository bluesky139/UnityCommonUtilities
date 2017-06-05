using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace common
{
	static class DynamicDllBuilder
	{
		static readonly string UNITY_TEMP_ASSEMBLY_DYNAMIC = LightningRebuild.WORKING_DIR + "/UnityTempFile-assembly-dynamic";

		[MenuItem("Common/Dynamic Dll/Build", priority = 100000)]
		static void Build()
		{
			Debug.Log("Build dynamic dll.");
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var types = assemblies.SelectMany(a => a.GetTypes())
				.Where(t => t.GetCustomAttributes(typeof(DynamicDllAttribute), false).Length > 0);
			List<string> filesToCompile = new List<string>();
			foreach (Type type in types)
			{
				var files = AssetDatabase.FindAssets(type + " t:Script")
					.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
					.Where(f => Path.GetFileNameWithoutExtension(f) == type.Name);
				Debug.AssertThrowException(files.Count() == 1, "Files of type " + type + " is not unique, \n" + "\n".Join(files));

				var file = files.First();
				filesToCompile.Add(file);
				Debug.Log("Type " + type + " -> " + file);
			}

			if (!File.Exists(LightningRebuild.UNITY_TEMP_ASSEMBLY))
			{
				throw new Exception("Can't find " + LightningRebuild.UNITY_TEMP_ASSEMBLY);
			}
			File.Copy(LightningRebuild.UNITY_TEMP_ASSEMBLY, UNITY_TEMP_ASSEMBLY_DYNAMIC, true);

			string apk = LightningRebuild.PrepareApk();
			Debug.Log("Use reference of Assembly-CSharp.dll and Assembly-CSharp-firstpass.dll from apk " + apk);
			_7z.Extract("tmp.apk", "assets/bin/Data/Managed/Assembly-CSharp.dll", LightningRebuild.WORKING_DIR);
			_7z.Extract("tmp.apk", "assets/bin/Data/Managed/Assembly-CSharp-firstpass.dll", LightningRebuild.WORKING_DIR);
			Utils.ReplaceFileLineByRegex(UNITY_TEMP_ASSEMBLY_DYNAMIC, @"\-r:[""']?Temp/Assembly\-CSharp\-firstpass\.dll[""']?$", 
				"-r:" + LightningRebuild.WORKING_DIR + "/Assembly-CSharp.dll\n-r:" + LightningRebuild.WORKING_DIR + "/Assembly-CSharp-firstpass.dll", true);

			Utils.DeleteFileLineByRegex(UNITY_TEMP_ASSEMBLY_DYNAMIC, @"[""']?Assets/.+.cs[""']?$");
			Utils.AppendFileLines(UNITY_TEMP_ASSEMBLY_DYNAMIC, filesToCompile);

			string outputWithTimestamp = LightningRebuild.WORKING_DIR + "/Assembly-CSharp-dynamic-" + MyTime.localUtcTimeStamp + ".dll";
			string output = LightningRebuild.WORKING_DIR + "/Assembly-CSharp-dynamic.dll";
			Utils.ReplaceFileLineByRegex(UNITY_TEMP_ASSEMBLY_DYNAMIC, @"\-out:[""']?Temp/Assembly\-CSharp\.dll[""']?$", 
				"-out:" + outputWithTimestamp, true);
			LightningRebuild.FixUnityBuiltInPath(UNITY_TEMP_ASSEMBLY_DYNAMIC);

			Debug.Log("Build dll.");
			Mono.CompileCSharpToDll(UNITY_TEMP_ASSEMBLY_DYNAMIC);
			if (File.Exists(output))
				File.Delete(output);
			File.Move(outputWithTimestamp, output);
			Debug.Log("Output: " + output);
		}
	}
}
