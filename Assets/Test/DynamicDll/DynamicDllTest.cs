using common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

class DynamicDllTest
{
	[FloatingDebug.Item("DynamicDll", "SetDll")]
	static void SetDll()
	{
		byte[] bytes = File.ReadAllBytes("/sdcard/Assembly-CSharp-dynamic.dll"); // You need download it from somewhere.
		DynamicDllLoader.SetDll(bytes);
	}

	[FloatingDebug.Item("DynamicDll", "Load")]
	static void Load()
	{
		DynamicDllLoader.Load("s1");
		DynamicDllLoader.Load("s2");
	}

	[FloatingDebug.Item("DynamicDll", "Unload")]
	static void Unload()
	{
		DynamicDllLoader.UnloadAll();
	}
}
