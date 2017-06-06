using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace common
{
	public class FPS
	{
		const float kFpsUpdateInterval = 0.5f;
		static float accum = 0f;
		static int frames = 0;

		static int fps_ = 0;
		public static int fps
		{
			get
			{
				return fps_;
			}
		}

		internal static void Update()
		{
			accum += Time.deltaTime;
			++frames;
			if (accum >= kFpsUpdateInterval)
			{
				fps_ = (int)(1f / (accum / frames));
				accum = 0f;
				frames = 0;
			}
		}
	}
}
