using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common
{
	public class MyTime
	{
		public static System.DateTime origin = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

		public static int localUtcTimeStamp
		{
			get
			{
				return (int)System.DateTime.UtcNow.Subtract(origin).TotalSeconds;
			}
		}
	}
}
