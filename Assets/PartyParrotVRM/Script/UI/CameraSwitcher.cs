using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PartyParrotVRM.Model;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace PartyParrotVRM.UI
{
    public class CameraSwitcher : MonoBehaviour
    {

        [SerializeField] private TMP_Dropdown dropdown;

        private string firstCameraId;
        private MediaDeviceInfo[] mediaDeviceInfos;
        public string CallbackMethodName
        {
            get
            {
                Action<string> callback = Receive;
                return callback.Method.Name;
            }
        }
        
        public string FirstCameraCallbackMethodName
        {
            get
            {
                Action<string> callback = FirstSelectedCamera;
                return callback.Method.Name;
            }
        }

        public class SelectClass : UnityEvent<string> { }

        public SelectClass OnValueChanged = new SelectClass();
        
        private void Start()
        {
#if UNITY_EDITOR
            firstCameraId = "f81981dc7b5c44a05401569d34d18eeeabde0d83e36a7d957cceefa459842285";
            using (var reader = new StreamReader($"{Define.MOCK_JSON_DIRECTORY}/mockDevices.json", Encoding.Default))
            {
                var text = reader.ReadToEnd();
                Receive(text);
            }
#endif
            dropdown.onValueChanged.AddListener(x =>
            {
                OnValueChanged.Invoke(mediaDeviceInfos[x].deviceId);
            });
        }

        public void Receive(string json)
        {
            var data = JSON.Parse(json);
            mediaDeviceInfos = new MediaDeviceInfo[data.Count];

            var index = 0;
            foreach(JSONNode deviceInfo in data.Values)
            {
                var info = mediaDeviceInfos[index] = new MediaDeviceInfo();
                info.deviceId = deviceInfo["deviceId"];
                info.kind = deviceInfo["kind"];
                info.groupId = deviceInfo["groupId"];
                info.label = deviceInfo["label"];
                index += 1;
            }

            dropdown.options.AddRange(mediaDeviceInfos.Select(x => new TMP_Dropdown.OptionData(x.label)));

            var selected = mediaDeviceInfos.FirstOrDefault(x => x.deviceId == firstCameraId);
            dropdown.value = mediaDeviceInfos.ToList().IndexOf(selected);
        }

        public void FirstSelectedCamera(string deviceId)
        {
            firstCameraId = deviceId;
        }
    }
}
