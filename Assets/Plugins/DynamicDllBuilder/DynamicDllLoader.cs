using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace common
{
	/// <summary>
	/// Class for dynamic dll load should inherit MonoBehaviour, 
	/// class name need same as file name,
	/// class name should be unique.
	/// </summary>
	public class DynamicDllAttribute : Attribute
	{
		public string name;
		public int priority; // Load first from smaller.
		public bool dontDestroyOnUnload; // Keep it on unload, only load once, it will be unload untill a different dll set.

		public DynamicDllAttribute(string name, int priority = 50, bool dontDestroyOnUnload = false)
		{
			this.name = name;
			this.priority = priority;
			this.dontDestroyOnUnload = dontDestroyOnUnload;
		}
	}

	public class DynamicDllLoader
	{
		private static Assembly assembly;
		private static string assemblyName;
		private static GameObject rootObject;

		public static void SetDll(byte[] bytes)
		{
		#if UNITY_ANDROID && !UNITY_EDITOR
			Debug.Log("DynamicDllLoader.SetDll()");
			string debugPath = Path.Combine(Application.persistentDataPath, "Assembly-CSharp-dynamic.dll");
			try
			{
				bytes = File.ReadAllBytes(debugPath);
				Debug.Log("SetDll from debug path " + debugPath);
			}
			catch {}

			if (bytes == null)
			{
				Debug.Log("Empty dynamic dll.");
				return;
			}

			try
			{
				Assembly newAssembly = Assembly.Load(bytes);
				if (newAssembly.FullName != assemblyName)
				{
					UnloadAll(true);
				}
				assembly = newAssembly;
				assemblyName = assembly.FullName;
				Debug.Log("Dynamic dll is loaded to assembly, length " + bytes.Length + ", name " + assemblyName);
			}
			catch
			{
				UnloadAll(true);
				assembly = null;
				Debug.LogError("Can't load dynamic dll to assembly.");
			}
		#endif
		}

		public static void Load(string name)
		{
		#if !UNITY_ANDROID || UNITY_EDITOR
			return;
		#endif
			Debug.Log("DynamicDllLoader.Load(" + name + ")");
			if (assembly == null)
			{
				Debug.LogError("Can't load " + name + " from dynamic all, assembly is null.");
				return;
			}

			IEnumerable<Type> types = assembly.GetTypes()
				.Where(type => 
				{
					DynamicDllAttribute[] attributes = type.GetCustomAttributes(typeof(DynamicDllAttribute), false) as DynamicDllAttribute[];
					return attributes.Length > 0 && attributes[0].name == name;
				})
				.OrderBy(type => (type.GetCustomAttributes(typeof(DynamicDllAttribute), false) as DynamicDllAttribute[])[0].priority);

			if (types.Count() == 0)
			{
				Debug.Log("No " + name + " in DynamicDllAttribute.");
				return;
			}

			if (rootObject == null)
			{
				rootObject = new GameObject("DynamicDll");
				GameObject.DontDestroyOnLoad(rootObject);
			}

			GameObject gameObject = null;
			for (int i = 0; i < rootObject.transform.childCount; ++i)
			{
				Transform child = rootObject.transform.GetChild(i);
				if (child.name == name)
				{
					gameObject = child.gameObject;
					break;
				}
			}
			if (gameObject == null)
			{
				gameObject = new GameObject(name);
				gameObject.transform.parent = rootObject.transform;
			}
			
			foreach (Type type in types)
			{
				if ((type.GetCustomAttributes(typeof(DynamicDllAttribute), false) as DynamicDllAttribute[])[0].dontDestroyOnUnload
					&& gameObject.GetComponent(type) != null)
				{
					Debug.Log("DynamicDll " + name + " " + type.Name + " is already loaded.");
					continue;
				}
				Debug.Log("DynamicDll  " + name + " load " + type.Name);
				gameObject.AddComponent(type);
			}
		}

		public static void UnloadAll(bool force = false)
		{
			if (rootObject == null)
				return;
			Debug.Log("DynamicDll UnloadAll(), force " + force);

			assembly = null;
			for (int i = 0; i < rootObject.transform.childCount; ++i)
			{
				GameObject child = rootObject.transform.GetChild(i).gameObject;
				MonoBehaviour[] behaviours = child.GetComponents<MonoBehaviour>();
				foreach (MonoBehaviour behaviour in behaviours)
				{
					Type type = behaviour.GetType();
					if (!force && (type.GetCustomAttributes(typeof(DynamicDllAttribute), false) as DynamicDllAttribute[])[0].dontDestroyOnUnload)
					{
						Debug.Log("DynamicDll keep " + child.name + " " + type.Name);
						continue;
					}
					Debug.Log("DynamicDll destroy " + child.name + " " + type.Name);
					Component.DestroyImmediate(behaviour);
				}
			}
		}
	}
}
