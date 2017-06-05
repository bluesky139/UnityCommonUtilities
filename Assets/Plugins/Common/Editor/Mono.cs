using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace common
{
	static class Mono
	{
		public static string UNITY_INSTALL_DIR
		{
			get
			{
				if (Application.platform != RuntimePlatform.WindowsEditor)
				{
					throw new Exception("Can't get UNITY_INSTALL_DIR, not work on " + Application.platform);
				}
				return Path.GetDirectoryName(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
			}
		}
		public static string MONO
		{
			get
			{
				return "\"" + UNITY_INSTALL_DIR + "/Editor/Data/Mono/bin/mono\"";
			}
		}
		public static string SMCS
		{
			get
			{
				return "\"" + UNITY_INSTALL_DIR + "/Editor/Data/Mono/lib/mono/unity/smcs.exe\"";
			}
		}

		public static void CompileCSharpToDll(string responseFile)
		{
			EditorUtils.ExecuteCmd(MONO + " " + SMCS + " -warn:0 @" + responseFile);
		}
	}
}
