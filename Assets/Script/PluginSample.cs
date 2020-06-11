using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PluginSample : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ExecuteJs(string id, string methodName, string callbackGameObjectName);
#endif

    void Start()
    {
        Execute("execute", "execute", "PluginSample");
    }

    private static void Execute(string id, string methodName, string callbackGameObjectName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ExecuteJs(id, methodName, callbackGameObjectName);
#endif
    }

    public void Callback(string message)
    {
        Debug.Log(message);
    }
}