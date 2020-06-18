using System.IO;
using System.Text;
using PartyParrotVRM.Provider;
using PartyParrotVRM.UI;
using Script.Model;
using Script.Parameter;
using UnityEngine;

namespace PartyParrotVRM.Controller
{
    public class ApplicationController : MonoBehaviour
    {

        [SerializeField] private Canvas canvas;
        [SerializeField] private Predictions predictions;
        [SerializeField] private AvatarController avatarController;
        [SerializeField] private CameraSwitcher cameraSwitcher;
        [SerializeField] private CameraPreview cameraPreview;
        [SerializeField] private DownloadGif downloadGif;
        [SerializeField] private VrmProvider vrmProvider;

        private readonly NativeExecuter executer = new NativeExecuter();
        private FaceMeshProvider faceMeshProvider;
    
        public Vector2 CameraScreen => new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        void Start()
        {
            faceMeshProvider = new FaceMeshProvider(predictions);
            avatarController.Injection(faceMeshProvider, this);
            cameraPreview.Injection(executer);
            downloadGif.Injection(executer, canvas, avatarController);
            vrmProvider.Injection(avatarController);

            var setupParameter = new SetupParameter
            {
                resolution = new[] { CameraScreen.x, CameraScreen.y },
                callbackGameObjectName = gameObject.name,
                callbackFunctionName = "CompleteSetup"
            };
            var setupParameterJson = JsonUtility.ToJson(setupParameter);
            executer.Execute("setup", setupParameterJson);

            cameraSwitcher.OnValueChanged.AddListener(x =>
            {
                var selectedCameraParameter = new SelectedCameraParameter
                {
                    selectedDeviceId = x,
                    resolution = new[] { CameraScreen.x, CameraScreen.y },
                    callbackGameObjectName = predictions.gameObject.name,
                    callbackFunctionName = predictions.CallbackMethodName
                };
                var selectedCameraParameterJson = JsonUtility.ToJson(selectedCameraParameter);
                executer.Execute("selectedCamera", selectedCameraParameterJson);
            });
        }

        public void CompleteSetup()
        {
            var callbackParameter = new CallbackParameter
            {
                callbackGameObjectName = predictions.gameObject.name,
                callbackFunctionName = predictions.CallbackMethodName
            };
            var parameterJson = JsonUtility.ToJson(callbackParameter);
            executer.Execute("trackingStart", parameterJson);
            
            var cameraListParameter = new CameraListParameter
            {
                callbackGameObjectName = cameraSwitcher.gameObject.name,
                callbackFunctionName = cameraSwitcher.CallbackMethodName,
                cameraSwitcherObject = cameraSwitcher.gameObject.name,
                firstCameraCallback = cameraSwitcher.FirstCameraCallbackMethodName
            };
            var cameraListParameterJson = JsonUtility.ToJson(cameraListParameter);
            executer.Execute("sendCameraList", cameraListParameterJson);
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            var index = Time.frameCount % 3;
            using (var reader = new StreamReader($"{Define.MOCK_JSON_DIRECTORY}/mock{index}.json", Encoding.Default))
            {
                var text = reader.ReadToEnd();
                predictions.Receive(text);
            }
            
            // using (var reader = new StreamReader($"{Define.MOCK_JSON_DIRECTORY}/mock{1}.json", Encoding.Default))
            // {
            //     var text = reader.ReadToEnd();
            //     predictions.Receive(text);
            // }
        }
#endif

    }
}
