using UnityEngine;

namespace PartyParrotVRM.Data
{
    [CreateAssetMenu(fileName = "CaptureModeTransform", menuName = "PartyParrotVRM/CaptureModeTransform")]
    public class CaptureModeTransform : ScriptableObject
    {
        public Vector3[] avatarPosition;
        public Vector3[] lookAtObjectPosition;
    }
}