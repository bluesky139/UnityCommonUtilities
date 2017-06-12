using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AirInspector;
using UnityEngine;

class ChildActivateTweaker : AirTweaker
{
    protected override Dictionary<string, object> GetSerializeObjs()
    {
        Dictionary<string, object> objs = new Dictionary<string, object>();
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            Transform child = gameObject.transform.GetChild(i);
            objs.Add(child.name, child.gameObject.activeSelf);
        }
        return objs;
    }

    protected override void SetFromSerializedObjs(Dictionary<string, object> objs)
    {
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (objs.ContainsKey(child.name))
            {
                child.gameObject.SetActive((bool) objs[child.name]);
            }
        }
    }
}
