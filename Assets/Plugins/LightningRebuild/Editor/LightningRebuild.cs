using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace common
{
	public static class LightningRebuild
	{
		public const string WORKING_DIR = "./_LightningRebuild";
		public const string UNITY_TEMP_ASSEMBLY = "./_LightningRebuild/UnityTempFile-assembly";
		public const string UNITY_TEMP_ASSEMBLY_FIRSTPASS = "./_LightningRebuild/UnityTempFile-assembly-firstpass";
		public const string TEMP_ASSEMBLY = "./Temp/Assembly-CSharp.dll";
		public const string TEMP_ASSEMBLY_FIRSTPASS = "./Temp/Assembly-CSharp-firstpass.dll";
		public const string APK_ASSEMBLY = "assets/bin/Data/Managed/Assembly-CSharp.dll";
		public const string APK_ASSEMBLY_FIRSTPASS = "assets/bin/Data/Managed/Assembly-CSharp-firstpass.dll";

		// C# files which will be copied after build, used for next lightning build.
		public static string[] specCSharpFiles;

		[MenuItem("Common/LightningRebuild/Rebuild C# (Android)")]
		static void RebuildCSharp_Android()
		{
			Debug.Log("RebuildCSharp (Android)");
			if (!Directory.Exists(WORKING_DIR))
				throw new Exception("Directory " + WORKING_DIR + " doesn't exist.");
			if (!File.Exists(UNITY_TEMP_ASSEMBLY))
				throw new Exception("File " + UNITY_TEMP_ASSEMBLY + " doesn't exist.");
			if (!File.Exists(UNITY_TEMP_ASSEMBLY_FIRSTPASS))
				throw new Exception("File " + UNITY_TEMP_ASSEMBLY_FIRSTPASS + " doesn't exist.");

			string apk = PrepareApk();
			Debug.Log("RebuildCSharp with apk " + apk);

			FixUnityBuiltInPath(UNITY_TEMP_ASSEMBLY);
			FixUnityBuiltInPath(UNITY_TEMP_ASSEMBLY_FIRSTPASS);

			Debug.Log("Build dll.");
			Mono.CompileCSharpToDll(UNITY_TEMP_ASSEMBLY_FIRSTPASS);
			Mono.CompileCSharpToDll(UNITY_TEMP_ASSEMBLY);
			if (Directory.Exists(WORKING_DIR + "/assets"))
				Directory.Delete(WORKING_DIR + "/assets", true);
			Directory.CreateDirectory(WORKING_DIR + "/" + Path.GetDirectoryName(APK_ASSEMBLY));
			File.Move(TEMP_ASSEMBLY, WORKING_DIR + "/" + APK_ASSEMBLY);
			File.Move(TEMP_ASSEMBLY + ".mdb", WORKING_DIR + "/" + APK_ASSEMBLY + ".mdb");
			File.Move(TEMP_ASSEMBLY_FIRSTPASS, WORKING_DIR + "/" + APK_ASSEMBLY_FIRSTPASS);
			File.Move(TEMP_ASSEMBLY_FIRSTPASS + ".mdb", WORKING_DIR + "/" + APK_ASSEMBLY_FIRSTPASS + ".mdb");

			Debug.Log("Replace dll.");
			string buildTools = EditorPrefs.GetString("AndroidSdkRoot") + "/build-tools";
			string buildTool = Directory.GetDirectories(buildTools).Last();
			string aapt = buildTool + "/aapt";
			EditorUtils.ExecuteCmd(aapt + " remove tmp.apk " + APK_ASSEMBLY, WORKING_DIR);
			EditorUtils.ExecuteCmd(aapt + " remove tmp.apk " + APK_ASSEMBLY_FIRSTPASS, WORKING_DIR);
			EditorUtils.ExecuteCmd(aapt + " list tmp.apk | grep \"" + APK_ASSEMBLY + "\" && aapt remove tmp.apk " + APK_ASSEMBLY + ".mdb", WORKING_DIR, ignoreError: true);
			EditorUtils.ExecuteCmd(aapt + " list tmp.apk | grep \"" + APK_ASSEMBLY_FIRSTPASS + "\" && aapt remove tmp.apk " + APK_ASSEMBLY_FIRSTPASS + ".mdb", WORKING_DIR, ignoreError: true);
			EditorUtils.ExecuteCmd(aapt + " add tmp.apk " + APK_ASSEMBLY, WORKING_DIR);
			EditorUtils.ExecuteCmd(aapt + " add tmp.apk " + APK_ASSEMBLY + ".mdb", WORKING_DIR);
			EditorUtils.ExecuteCmd(aapt + " add tmp.apk " + APK_ASSEMBLY_FIRSTPASS, WORKING_DIR);
			EditorUtils.ExecuteCmd(aapt + " add tmp.apk " + APK_ASSEMBLY_FIRSTPASS + ".mdb", WORKING_DIR);

			Debug.Log("Resign.");
			EditorUtils.ExecuteCmd(aapt + " remove tmp.apk META-INF/CERT.RSA", WORKING_DIR, ignoreError: true);
			EditorUtils.ExecuteCmd(aapt + " remove tmp.apk META-INF/CERT.SF", WORKING_DIR, ignoreError: true);
			EditorUtils.ExecuteCmd("jarsigner -verbose -digestalg SHA1 -sigalg MD5withRSA -keystore \"" 
				+ EditorEnv.externalRoot + "/keystore/android.keystore\"" 
				+ " -storepass 123456 -signedjar unaligned.apk tmp.apk android.keystore", 
                WORKING_DIR);

			Debug.Log("Zip align.");
			string zipAlign = buildTool + "/zipalign";
			EditorUtils.ExecuteCmd(zipAlign + " -f -v 4 unaligned.apk rebuilt.apk", WORKING_DIR);
			Debug.Log("Output: " + WORKING_DIR + "/rebuilt.apk");
		}

		public static string PrepareApk()
		{
			string[] files = Directory.GetFiles(".", "*.apk");
			Debug.AssertThrowException(files.Length == 1, "Expect 1 apk, but " + files.Length);
			string apk = files[0];
			File.Copy(apk, WORKING_DIR + "/tmp.apk", true);
			return apk;
		}

		public static void FixUnityBuiltInPath(string file)
		{
			Debug.Log("Fix Unity built-in dll path for " + file);
			string[] lines = File.ReadAllLines(file);
			lines = lines.Select(line =>
			{
				if (line.StartsWith("-r:"))
				{
					Regex reg = new Regex("^-r:[\"'](.+)[\"']$");
					Match match = reg.Match(line);
					if (!match.Success)
						return line;
					string path = match.Groups[1].Value.Replace('\\', '/');

					var pathRegs = new Dictionary<string, string>()
					{
						{ "(\\w:/.+/Unity)(/Editor/Data/.+\\.dll)", Mono.UNITY_INSTALL_DIR },
						{ "(/.+/Unity)(/PlaybackEngines/AndroidPlayer/.+\\.dll)", Mono.UNITY_INSTALL_DIR + "/Editor/Data" },
						{ "(/.+/Unity/Unity.app/Contents/UnityExtensions)(/Unity/.+\\.dll)", Mono.UNITY_INSTALL_DIR + "/Editor/Data/UnityExtensions" },
						{ "(/.+/Unity/Unity.app/Contents/Frameworks)(/Mono/lib/)", Mono.UNITY_INSTALL_DIR + "/Editor/Data" }
					};
					foreach (KeyValuePair<string, string> pathReg in pathRegs)
					{
						string newPath = Regex.Replace(path, pathReg.Key, pathReg.Value + "$2");
						if (newPath != path)
						{
							string newLine = "-r:\"" + newPath + "\"";
							Debug.Log(line + " -> " + newLine);
							return newLine;
						}
					}
				}
				return line;
			})
			.ToArray();
			File.WriteAllLines(file, lines);
		}

		[MenuItem("Common/LightningRebuild/Rebuild C# And Install (Android)")]
		static void RebuildCSharp_Install_Android()
		{
			RebuildCSharp_Android();
			InstallApk(WORKING_DIR + "/rebuilt.apk");
		}

		static void InstallApk(string target)
        {
            string sdk = EditorPrefs.GetString("AndroidSdkRoot");
            string adb = sdk + "/platform-tools/adb.exe";
            string adbParam = "install -r " + target;
            System.Diagnostics.Process.Start("cmd.exe", "/C echo adb " + adbParam + " & " + adb + " " + adbParam + " & pause");
        }

		public static void PreBuild(BuildTarget build_target)
		{
			if (build_target != BuildTarget.Android)
				return;

			Debug.Log("LightningRebuild PreBuild");
			if (Directory.Exists(WORKING_DIR))
				Directory.Delete(WORKING_DIR, true);

			string[] files = Directory.GetFiles("./Temp", "UnityTempFile-*");
			foreach (string file in files)
			{
				File.Delete(file);
			}
		}

		[PostProcessBuild]
		static void PostBuild(BuildTarget target, string pathToBuiltProject)
		{
			if (target != BuildTarget.Android)
				return;

			try
			{
				DoPostBuild();
			} 
			catch (Exception e)
			{
				Debug.LogError("LightningRebuild PostBuild error, " + e.Message + "\n" + e.StackTrace);
			}
		}

		static void DoPostBuild()
		{
			Debug.Log("LightningRebuild PostBuild");
			string[] files = Directory.GetFiles("./Temp", "UnityTempFile-*");
			Debug.Assert(files.Length == 2 || files.Length == 3, "Expect 2 or 3 UnityTempFiles, but " + files.Length + ", lightning rebuild won't work after this build.");
			
			Func<string, string> GetFileName = delegate (string file)
			{
				string[] lines = File.ReadAllLines(file);
				if (lines.ContainsRegex(@"-out:[""']?Temp/Assembly-CSharp.dll[""']?"))
				{
					return UNITY_TEMP_ASSEMBLY;
				}
				if (lines.ContainsRegex(@"-out:[""']?Temp/Assembly-CSharp-firstpass.dll[""']?"))
				{
					return UNITY_TEMP_ASSEMBLY_FIRSTPASS;
				}
				if (lines.ContainsRegex(@"-out:[""']?Temp/Assembly-UnityScript.dll[""']?"))
				{
					return null;
				}
				File.Copy(file, WORKING_DIR + "/" + Path.GetFileName(file));
				throw new Exception("Not a valid mono build file, " + file + ", " + "\n".Join(lines));
			};

			if (!Directory.Exists(WORKING_DIR))
				Directory.CreateDirectory(WORKING_DIR);

			int gotCount = 0;
			foreach (string file in files)
			{
				string dstFile = GetFileName(file);
				if (dstFile == null)
					continue;
				File.Move(file, dstFile);
				Debug.Log("Got " + dstFile + " for next lightning rebuild.");
				++gotCount;
			}
			Debug.AssertThrowException(gotCount == 2, "Can't get " + UNITY_TEMP_ASSEMBLY + " or " + UNITY_TEMP_ASSEMBLY_FIRSTPASS);

			if (specCSharpFiles != null)
			{
				foreach (string file in specCSharpFiles)
				{
					Debug.Log("Backup " + file);
					Debug.AssertThrowException(file.StartsWith("Assets/"), file + " should starts with \"Assets/\" in LightningRebuild.specCSharpFiles");
					string dst = WORKING_DIR + "/spec_files/" + file;
					Directory.CreateDirectory(Path.GetDirectoryName(dst));
					File.Copy(file, dst);
					Utils.ReplaceFilesLineByRegex(new string[] { UNITY_TEMP_ASSEMBLY, UNITY_TEMP_ASSEMBLY_FIRSTPASS }, 
						string.Format(@"[""']?{0}[""']?$", file.Replace(".", @"\.")), WORKING_DIR + "/spec_files/" + file, 1, true);
				}
			}

			if (File.Exists("./Assets/smcs.rsp"))
			{
				Debug.Log("Backup smcs.rsp");
				File.Copy("./Assets/smcs.rsp", WORKING_DIR + "/smcs.rsp", true);
				Utils.ReplaceFilesLineByRegex(new string[] { UNITY_TEMP_ASSEMBLY, UNITY_TEMP_ASSEMBLY_FIRSTPASS }, 
					@"[""']?@Assets[/\\]smcs\.rsp[""']?$", "@" + WORKING_DIR + "/smcs.rsp", 2, true);
			}

			Debug.Log("Fix Assembly-CSharp-firstpass.dll reference in " + UNITY_TEMP_ASSEMBLY);
			Utils.ReplaceFileLineByRegex(UNITY_TEMP_ASSEMBLY, @"^\-r:[""']?Library/ScriptAssemblies/Assembly\-CSharp\-firstpass\.dll[""']?$", "-r:Temp/Assembly-CSharp-firstpass.dll", true);

			Debug.Log("LightningRebuild PostBuild end.");
		}
	}
}
