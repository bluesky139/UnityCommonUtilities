using UnityEngine;
using UnityEditor;
using System.Collections;

namespace common
{ 

public class EditorCoroutine // only support yield return null
{
    public static EditorCoroutine Start(IEnumerator routine)
    {
        EditorCoroutine coroutine = new EditorCoroutine(routine);
        coroutine.Start();
        return coroutine;
    }

    IEnumerator routine;
    EditorCoroutine(IEnumerator routine)
    {
        this.routine = routine;
    }

    void Start()
    {
        EditorApplication.update += Update;
    }

    void Stop()
    {
        EditorApplication.update -= Update;
    }

    void Update()
    {
        if (!routine.MoveNext())
            Stop();
    }
}

}