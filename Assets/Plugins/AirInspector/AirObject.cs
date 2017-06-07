using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AirInspector
{
    class AirObject
    {
        public string name { get; private set; }
        public int    id   { get; private set; }

        public AirObject(string name, int id)
        {
            this.name = name;
            this.id   = id;
        }

        public static List<AirObject> FindRootObjects()
        {
            List<AirObject> airObjs = new List<AirObject>();
            GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objs)
            {
                if (obj.transform.parent == null)
                {
                    AirObject airObj = new AirObject(obj.name, obj.GetInstanceID());
                    airObjs.Add(airObj);
                }
            }
            return airObjs;
        }
    }
}
