using System.Runtime.InteropServices;
using UnityEngine;

namespace PartyParrotVRM
{
    public class NativeExecuter
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void execute(string methodName, string parameter);
#endif

        public void Execute(string methodName, string parameter = "{}")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            execute(methodName, parameter);
#else
            Debug.Log($"call native method: {methodName}, parameter : {parameter}");
#endif
        }
    }
}
