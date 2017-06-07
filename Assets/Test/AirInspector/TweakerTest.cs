using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AirInspector;

class TweakerTest : AirTweaker
{
	protected override Dictionary<string, object> GetSerializeObjs()
	{
		var objs = new Dictionary<string, object>();
		var behaviour = GetComponent<TweakerTestBehaviour>();
		objs.Add("maxNum", behaviour.maxNum);
		objs.Add("maxFloat", behaviour.maxFloat);
		objs.Add("aString", behaviour.aString);
		return objs;
	}

	protected override void SetFromSerializedObjs(Dictionary<string, object> objs)
	{
		var behaviour = GetComponent<TweakerTestBehaviour>();
		foreach (var kv in objs)
		{
			if (kv.Key == "maxNum")
			{
				behaviour.maxNum = (int) kv.Value;
			}
			else if (kv.Key == "maxFloat")
			{
				behaviour.maxFloat = (float) kv.Value;
			} 
			else if (kv.Key == "aString")
			{
				behaviour.aString = (string) kv.Value;
			}
		}
	}
}
