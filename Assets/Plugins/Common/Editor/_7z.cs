using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common
{
	static class _7z
	{
		static readonly String z = EditorEnv.externalRoot + "/7z/7z";

		public static void Extract(string zipPath, string filePath, string workingDir)
		{
			EditorUtils.ExecuteCmd(z + " e -y " + zipPath + " " + filePath, workingDir);
		}
	}
}
