using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Script.Model;
using UniGLTF;
using VRM;

namespace PartyParrotVRM.Provider
{
    public class FaceMeshProvider
    {

        // https://raw.githubusercontent.com/tensorflow/tfjs-models/master/facemesh/mesh_map.jpg
        private Predictions predictions;

        public Vector3 FirstScaledMesh => predictions.scaledMesh.FirstOrDefault();

        private static Dictionary<BlendShapePreset, Vector2> Vowels = new Dictionary<BlendShapePreset, Vector2>
        {
            { BlendShapePreset.A, new Vector2(40, 75) },
            { BlendShapePreset.I, new Vector2(17, 90) },
            { BlendShapePreset.U, new Vector2(6, 58) },
            { BlendShapePreset.E, new Vector2(25, 82) },
            { BlendShapePreset.O, new Vector2(14, 52) },
        };

        public Vector3 LookTargetPosition
        {
            get
            {
                if (predictions.scaledMesh.Length == 0)
                {
                    return Vector3.zero;
                }
                var center = predictions.scaledMesh[0];
                var left = predictions.scaledMesh[37];
                var right = predictions.scaledMesh[267];
                var up = predictions.scaledMesh[167];
                var down = predictions.scaledMesh[11];
                var centerPoint = (center + left + right + up + down) / 5f;
                return centerPoint;
            }
        }

        public ValueTuple<BlendShapeKey, float> MouthShape
        {
            get
            {
                var blendShape = BlendShapePreset.Unknown;
                if (predictions.scaledMesh.Length == 0)
                {
                    return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(blendShape), 0f);
                }
                var left = predictions.scaledMesh[78];
                var right = predictions.scaledMesh[308];
                var top = predictions.scaledMesh[13];
                var bottom = predictions.scaledMesh[14];
                var verticalScale = Mathf.Abs(top.y - bottom.y);
                var horizontalScale = Mathf.Abs(left.x - right.x);
                var vec = new Vector2(horizontalScale, verticalScale);

                var approximationIndex = 0;
                var min = 1000f;
                var vowelsArray = Vowels.ToArray();
                for (var i = 0; i < vowelsArray.Length; i++)
                {
                    var distance = Vector2.Distance(vowelsArray[i].Value, vec);
                    if (distance < min)
                    {
                        approximationIndex = i;
                        min = distance;
                    }
                }

                var vowel = vowelsArray[approximationIndex];
                var key = vowel.Key;
                var value = 1f - min / 100f;
                var isMouthClose = value < 0.2f;
                if (isMouthClose) key = BlendShapePreset.Unknown;
                return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(key), Mathf.Clamp01(value));
            }
        }
        
        public ValueTuple<BlendShapeKey, float> LeftEyeShape
        {
            get
            {
                var blendShape = BlendShapePreset.Blink_L;
                if (predictions.scaledMesh.Length == 0) return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(blendShape), 0f);
                var top = predictions.scaledMesh[386].y;
                var bottom = predictions.scaledMesh[374].y;
                var distance = Mathf.Abs(top - bottom) / 20f;
                return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(blendShape), Mathf.Clamp01(1f - distance));
            }
        }
        
        public ValueTuple<BlendShapeKey, float> RightEyeShape
        {
            get
            {
                var blendShape = BlendShapePreset.Blink_R;
                if (predictions.scaledMesh.Length == 0) return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(blendShape), 0f);
                var top = predictions.scaledMesh[159].y;
                var bottom = predictions.scaledMesh[145].y;
                var distance = Mathf.Abs(top - bottom) / 20f;
                return new ValueTuple<BlendShapeKey, float>(new BlendShapeKey(blendShape), Mathf.Clamp01(1f - distance));
            }
        }

        

        public FaceMeshProvider(Predictions predictions)
        {
            this.predictions = predictions;
        }

    }
}
