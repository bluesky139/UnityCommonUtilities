using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MacroTest : MonoBehaviour
{
	void Awake()
	{
	#if RELEASE_VERSION
		Debug.Log("This is release verison.");
	#else
		Debug.Log("This is debug verison.");
	#endif
	}
}
