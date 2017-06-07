using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TweakerTestBehaviour : MonoBehaviour
{
	private int maxNum_ = 7;
	private float maxFloat_ = 29.9f;
	private string aString_ = "a value";

	public int maxNum
	{
		get
		{
			return maxNum_;
		}
		set
		{
			Debug.Log("TweakerTestBehaviour set maxNum " + value);
			maxNum_ = value;
		}
	}

	public float maxFloat
	{
		get
		{
			return maxFloat_;
		}
		set
		{
			Debug.Log("TweakerTestBehaviour set maxFloat " + value);
			maxFloat_ = value;
		}
	}

	public string aString
	{
		get
		{
			return aString_;
		}
		set
		{
			Debug.Log("TweakerTestBehaviour set aString " + value);
			aString_ = value;
		}
	}
}
