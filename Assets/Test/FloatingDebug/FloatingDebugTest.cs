using common;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

class FloatingDebugTest : MonoBehaviour
{
	void Awake()
	{
		FloatingDebug.Create();
	}

	[FloatingDebug.Item("FloatingDebug", "Test")]
	static void Test()
	{
		common.Debug.Log("FloatingDebugTest.");
	}

	[FloatingDebug.Item("FloatingDebug", "Text", FloatingDebug.ItemAttribute.Type.TextInfo)]
	static string Text()
	{
		return "Test text;";
	}

	[FloatingDebug.Item("FloatingDebug", "MoreButtons", FloatingDebug.ItemAttribute.Type.MoreButtons)]
	static SortedDictionary<string, IGrouping<string, KeyValuePair<string, MethodInfo>>> MoreButtons()
	{
		MethodInfo methodCall = typeof(MiddleClass).GetMethod("_Call", BindingFlags.Static | BindingFlags.NonPublic);
		var itemMethods = typeof(InternalClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Select(m => new KeyValuePair<string, MethodInfo>(m.Name, methodCall))
			.GroupBy(m => "Test")
			.ToDictionary(kv => kv.Key);
		return new SortedDictionary<string, IGrouping<string, KeyValuePair<string, MethodInfo>>>(itemMethods);
	}

	class MiddleClass
	{
		static void _Call(string methodName)
		{
			MethodInfo method = typeof(MiddleClass).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				method = typeof(InternalClass).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
				method.Invoke(null, null);
			}
			else
			{
				method.Invoke(null, null);
			}
		}

		public static void Method3()
		{
			InternalClass.Method3("a");
		}
	}

	class InternalClass
	{
		public static void Method1()
		{
			common.Debug.Log("InternalClass Method1");
		}

		public static void Method2()
		{
			common.Debug.Log("InternalClass Method2");
		}

		public static void Method3(string s)
		{
			common.Debug.Log("InternalClass Method3");
		}
	}
}