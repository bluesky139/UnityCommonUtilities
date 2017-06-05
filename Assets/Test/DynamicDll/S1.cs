using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[common.DynamicDll("s1")]
class S1 : MonoBehaviour
{
	float lastTime;

	void Awake()
	{
		Debug.Log("S1 Awake() 2");
		lastTime = Time.realtimeSinceStartup;
	}

	void Update()
	{
		if (Time.realtimeSinceStartup - lastTime > 2)
		{
			Debug.Log("S1 Update() 2");
			lastTime = Time.realtimeSinceStartup;
		}
	}
}
