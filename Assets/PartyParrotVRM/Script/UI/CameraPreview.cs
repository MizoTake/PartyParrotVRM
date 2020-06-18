using UnityEngine;
using UnityEngine.UI;

namespace PartyParrotVRM.UI
{
    public class CameraPreview : MonoBehaviour
    {

        [SerializeField] private Button previewButton;
        [SerializeField] private Image buttonImage;

        private NativeExecuter executer;
        private bool isPressed;
        private Color normal;
        private Color pressed;
        
        public void Injection(NativeExecuter executer)
        {
            this.executer = executer;
        }
    
        void Start()
        {
            normal = previewButton.colors.normalColor;
            pressed = previewButton.colors.pressedColor;

            buttonImage.color = pressed;
            previewButton.onClick.AddListener(() =>
            {
                executer.Execute("cameraPreview");
                isPressed = !isPressed;
                buttonImage.color = isPressed ? pressed : normal;
            });
        }
    }
}
