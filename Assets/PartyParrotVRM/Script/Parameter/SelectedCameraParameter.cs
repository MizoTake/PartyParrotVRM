using System;

namespace Script.Parameter
{
    [Serializable]
    public class SelectedCameraParameter : CallbackParameter
    {
        public string selectedDeviceId;
        public float[] resolution;
    }
}