using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[common.DynamicDll("s2")]
class S2 : MonoBehaviour
{
	float lastTime;

	void Awake()
	{
		Debug.Log("S2 Awake() 2");
		lastTime = Time.realtimeSinceStartup;
	}

	void Update()
	{
		if (Time.realtimeSinceStartup - lastTime > 2)
		{
			Debug.Log("S2 Update() 2");
			lastTime = Time.realtimeSinceStartup;
		}
	}
}
