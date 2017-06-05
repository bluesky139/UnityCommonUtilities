using common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

class DynamicDllTest : MonoBehaviour
{
	void Awake()
	{
		DynamicDllLoader.SetDll(File.ReadAllBytes("/sdcard/a.dll"));
		DynamicDllLoader.Load("s1");
	}
}
