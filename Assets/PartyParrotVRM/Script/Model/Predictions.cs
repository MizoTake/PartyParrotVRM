using System;
using SimpleJSON;
using UnityEngine;

namespace Script.Model
{
    public class Predictions : MonoBehaviour
    {
        public float faceInViewConfidence;
        public BoundingBox boundingBox;
        public Vector3[] mesh;
        public Vector3[] scaledMesh;
        public Annotations annotations;

        public string CallbackMethodName
        {
            get
            {
                Action<string> callback = Receive;
                return callback.Method.Name;
            }
        }

        [Serializable]
        public class BoundingBox
        {
            public float[] topLeft;
            public float[] bottomRight;
        }

        [Serializable]
        public class Annotations
        {
            public Vector3[] silhouette;
        }
        
        public void Receive(string json)
        {
            // 顔が複数認識されたときになにかあるかも
            json = json.Remove(0, 1).Remove(json.Length - 2, 1);
        
            var data = JSON.Parse(json);
            faceInViewConfidence = float.Parse(data["faceInViewConfidence"].Value);
            var scaledMeshSource = data["scaledMesh"];
            scaledMesh = new Vector3[scaledMeshSource.Count];
            var index = 0;
            foreach(JSONNode n in scaledMeshSource)
            {
                var x = float.Parse(n[0].Value);
                var y = float.Parse(n[1].Value);
                var z = float.Parse(n[2].Value);
                scaledMesh[index] = new Vector3(x, y, z);
                index += 1;
            }
        }
    }
}