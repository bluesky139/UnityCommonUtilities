using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace common
{
public class InstanceTemplate<T> where T : InstanceTemplate<T>
{
	static readonly object locker = new object();

	private static T instance_;
	public static T instance
	{
		get
		{
			if (instance_ == null)
			{
				lock (locker)
				{
					if (instance_ == null)
						instance_ = Activator.CreateInstance<T>();
				}
			}
			return instance_;
		}
    }

	public static bool hasInstance
	{
		get
		{
			return instance_ != null;
		}
	}

	public static void DestoryInstance()
	{
		lock (locker)
		{
			if (instance_ != null)
			{
				instance_.OnDestroy();
				instance_ = default(T);
			}
		}
    }

	protected virtual void OnDestroy()
	{
	}
}
}