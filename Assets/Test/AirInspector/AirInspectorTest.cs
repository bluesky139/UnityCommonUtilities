using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AirInspector;

class AirInspectorTest : MonoBehaviour
{
	void Update()
	{
		AirMgr.instance.Update();
	}
}
