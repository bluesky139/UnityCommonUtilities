using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[common.DynamicDll("s1", priority = 2, dontDestroyOnUnload = true)]
class S1B : MonoBehaviour
{
	float lastTime;

	void Awake()
	{
		Debug.Log("S1B Awake() 2");
		lastTime = Time.realtimeSinceStartup;
	}

	void Update()
	{
		if (Time.realtimeSinceStartup - lastTime > 2)
		{
			Debug.Log("S1B Update() 2");
			lastTime = Time.realtimeSinceStartup;
		}
	}
}
