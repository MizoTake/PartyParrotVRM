using System;
using System.Collections;
using PartyParrotVRM.Controller;
using PartyParrotVRM.Parameter;
using Script.Parameter;
using UnityEngine;
using UnityEngine.UI;

namespace PartyParrotVRM.UI
{
    public class DownloadGif : MonoBehaviour
    {

        private const int CAPTURE_COUNT = 10;
        
        [SerializeField] private Button downloadGifButton;
        private NativeExecuter executer;

        private int captureIndex = 0;
        private Canvas canvas;
        private AvatarController avatarController;
        
        public string CallbackMethodName
        {
            get
            {
                Action callback = StartNotify;
                return callback.Method.Name;
            }
        }
        public void Injection(NativeExecuter executer, Canvas canvas, AvatarController avatarController)
        {
            this.executer = executer;
            this.canvas = canvas;
            this.avatarController = avatarController;
        }
    
        void Start()
        {
            downloadGifButton.onClick.AddListener(() =>
            {
                var callbackParameter = new CallbackParameter
                {
                    callbackGameObjectName = gameObject.name,
                    callbackFunctionName = CallbackMethodName
                };
                var parameterJson = JsonUtility.ToJson(callbackParameter);
                executer.Execute("downloadStartGif", parameterJson);
                canvas.enabled = false;
                avatarController.StartCaptureMode();
#if UNITY_EDITOR
                StartNotify();
#endif
            });
        }

        public void StartNotify()
        {
            StartCoroutine(CaptureScreen());
        }

        IEnumerator CaptureScreen()
        {
            while (captureIndex < CAPTURE_COUNT)
            {
                avatarController.SetCaptureCut(captureIndex);
                yield return new WaitForEndOfFrame();
                captureIndex += 1;
                var captureParameter = new CaptureParameter
                {
                    index = captureIndex,
                    max = CAPTURE_COUNT
                };
                var parameterJson = JsonUtility.ToJson(captureParameter);
                executer.Execute("captureFrame", parameterJson);
            }
            captureIndex = 0;
            canvas.enabled = true;
            avatarController.FinishCaptureMode();
        }
    }
}
