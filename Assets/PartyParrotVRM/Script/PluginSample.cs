using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Script.Model;
using Script.Parameter;
using SimpleJSON;
using UnityEngine;
using VRM;

public class PluginSample : MonoBehaviour
{
    private static string MOCK_JSON_DIRECTORY => $"{Application.dataPath}/PartyParrotVRM/StreamingAssets";

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void execute(string methodName, string parameter);
#endif
    
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parent;
    // [SerializeField] private VRMLookAtHead lookHead;
    // [SerializeField] private Transform avator;
    
    private bool canUpdateFace = false;
    private bool isFirstPosition = true;
    
    private List<GameObject> faceParts = new List<GameObject>();

    void Start()
    {
        // var setupParameter = new SetupParameter
        // {
        //     resolution = new float[] { Screen.width, Screen.height },
        //     callbackGameObjectName = gameObject.name,
        //     callbackFunctionName = "CompleteSetup"
        // };
        // var setupParameterJson = JsonUtility.ToJson(setupParameter);
        // Execute("setup", setupParameterJson);
    }

    private void Execute(string methodName, string parameter)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        execute(methodName, parameter);
#endif
#if UNITY_EDITOR
        // for (int i = 0; i < 3; i++)
        // {
        //     using (var reader = new StreamReader($"{MOCK_JSON_DIRECTORY}/mock{i}.json", Encoding.Default))
        //     {
        //         var text = reader.ReadToEnd();
        //         FaceMesh(text);
        //     }
        // }

        using (var reader = new StreamReader($"{MOCK_JSON_DIRECTORY}/mock{1}.json", Encoding.Default))
        {
            var text = reader.ReadToEnd();
            FaceMesh(text);
        }
#endif
    }

    private int testIndex = 0;
    private void Update()
    {
#if UNITY_EDITOR

        var index = Time.frameCount % 3;
        Debug.Log(Time.frameCount + " " + index);
        if (testIndex == 3) testIndex = 0;
        using (var reader = new StreamReader($"{MOCK_JSON_DIRECTORY}/mock{testIndex}.json", Encoding.Default))
        {
            var text = reader.ReadToEnd();
            FaceMesh(text);
        }
#endif
    }

    public void CompleteSetup()
    {
        var callbackParameter = new CallbackParameter()
        {
            callbackGameObjectName = gameObject.name,
            callbackFunctionName = "FaceMesh"
        };
        var parameterJson = JsonUtility.ToJson(callbackParameter);
        Execute("trackingStart", parameterJson);
    }

    public void FaceMesh(string json)
    {
        testIndex += 1;
        
        json = json.Remove(0, 1).Remove(json.Length - 2, 1);
        
        var data = JSON.Parse(json);
        var faceInViewConfidence = float.Parse(data["faceInViewConfidence"].Value);
        var scalemesh = data["scaledMesh"];
        var index = 0;
        foreach(JSONNode n in scalemesh)
        {
            if (faceParts.Count <= index)
            {
                faceParts.Add(Instantiate(prefab, parent.transform));
            }
            var obj = faceParts[index];
            obj.name = $"{index} : mesh part";
            
            var x = float.Parse(n[0].Value);
            var y = float.Parse(n[1].Value);
            var z = float.Parse(n[2].Value);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.localScale = Vector3.one * 10f;

            index += 1;
        }
        
        var firstObject = faceParts.FirstOrDefault();

        // if (isFirstPosition)
        // {
        //     avator.transform.position = new Vector3(avator.transform.position.x, firstObject.transform.position.y, avator.transform.position.z);
        //     Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, firstObject.transform.position.y + 1.5f, Camera.main.transform.position.z);
        //
        //     isFirstPosition = false;
        // }
        //
        // if (firstObject != null) lookHead.Target = firstObject.transform;
        // lookHead.Head.LookAt(firstObject.transform);
        
        if (scalemesh.Count <= faceParts.Count)
        {
            faceParts.RemoveRange(faceParts.Count, scalemesh.Count - faceParts.Count);
        }
    }
}