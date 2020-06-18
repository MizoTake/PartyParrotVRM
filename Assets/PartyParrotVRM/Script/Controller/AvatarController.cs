using System.Collections.Generic;
using PartyParrotVRM.Component;
using PartyParrotVRM.Data;
using PartyParrotVRM.Provider;
using UnityEngine;
using VRM;

namespace PartyParrotVRM.Controller
{
    public class AvatarController : MonoBehaviour
    {

        [SerializeField] private CaptureModeTransform captureModeTransform;
        [SerializeField] private Transform avatar;
        [SerializeField] private Transform lookAtTarget;
        [SerializeField] private AnimatorOverrideController controller;

        private FaceMeshProvider faceMeshProvider;
        private ApplicationController application;
        private VRMBlendShapeProxy shapeProxy;
        private List<Material> avatarMaterials = new List<Material>();

        private Vector3 cacheLookAtPosition;
        private bool captureMode = false;
        private Transform head;
        
        private const float alpha = 0.8f;
        private int colorIndex = 0;
        private Color[] partyParrotColors =
        {
            new Color(1, 1, 1, alpha), 
            new Color(1, 0.87f, 0.58f, alpha),
            new Color(0.56f, 1, 0.58f, alpha), 
            new Color(0.56f, 1, 1, alpha),
            new Color(0.56f, 0.74f, 1, alpha),
            new Color(0.86f, 0.56f, 1, alpha),
            new Color(1, 0.56f, 1, alpha),
            new Color(1, 0.50f, 1, alpha),
            new Color(1, 0.42f, 0.74f, alpha),
            new Color(1, 0.42f, 0.43f, alpha), 
        };

        private BlendShapePreset[] mouthShapes =
        {
            BlendShapePreset.A,
            BlendShapePreset.I,
            BlendShapePreset.U,
            BlendShapePreset.E,
            BlendShapePreset.O
        };

        public void Injection(FaceMeshProvider faceMeshProvider, ApplicationController application)
        {
            this.faceMeshProvider = faceMeshProvider;
            this.application = application;
        }

        private void Start()
        {
            shapeProxy = avatar.GetComponent<VRMBlendShapeProxy>();
            head = avatar.GetComponent<VRMFirstPerson>().FirstPersonBone;
            SettingsPartyMaterial();
        }

        private void Update()
        {
            if (captureMode) return;
            var centerPos = faceMeshProvider.LookTargetPosition - new Vector3(application.CameraScreen.x/2f, application.CameraScreen.y/2f, 0);
            centerPos /= 100f;
            var nextPos = new Vector3(centerPos.x, -centerPos.y, centerPos.z + 10f);
            lookAtTarget.position = Vector3.Lerp(lookAtTarget.position, nextPos, Time.deltaTime);

            foreach (var mat in avatarMaterials)
            {
                mat.color = partyParrotColors[colorIndex];
            }
            colorIndex += 1;
            if (colorIndex >= partyParrotColors.Length) colorIndex = 0;

            var (blendShape, value) = faceMeshProvider.MouthShape;
            foreach (var shape in mouthShapes)
            {
                if (blendShape.Preset == BlendShapePreset.Unknown && shape != blendShape.Preset)
                {
                    shapeProxy.ImmediatelySetValue(shape, 0f);
                }
                else
                {
                    shapeProxy.ImmediatelySetValue(blendShape, value);
                }
            }
            var (eyeLeft, eyeLeftValue) = faceMeshProvider.LeftEyeShape;
            shapeProxy.ImmediatelySetValue(eyeLeft, eyeLeftValue);
            var (eyeRight, eyeRightValue) = faceMeshProvider.RightEyeShape;
            shapeProxy.ImmediatelySetValue(eyeRight, eyeRightValue);
        }

        private void SettingsPartyMaterial()
        {
            avatarMaterials.Clear();
            foreach (var renderer in avatar.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                avatarMaterials.AddRange(renderer.materials);
            }
        }

        public void ChangeAvatar(GameObject nextAvatar)
        {
            nextAvatar.transform.SetPositionAndRotation(avatar.position, avatar.rotation);
            if (avatar != null)
            {
                avatar.transform.position = Vector3.down * -1000f;
                Destroy(avatar.gameObject);
            }
            avatar = nextAvatar.transform;
            avatar.parent = transform;
            shapeProxy = avatar.GetComponent<VRMBlendShapeProxy>();
            head = avatar.GetComponent<VRMFirstPerson>().FirstPersonBone;
            var blendAnimation = avatar.gameObject.AddComponent<BlendAnimation>();
            var animator = avatar.gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            blendAnimation.Injection(animator, lookAtTarget);
            SettingsPartyMaterial();
        }
        
        public void StartCaptureMode()
        {
            captureMode = true;
            cacheLookAtPosition = lookAtTarget.transform.position;
        }
        
        public void FinishCaptureMode()
        {
            captureMode = false;
            avatar.transform.position = Vector3.zero;
            lookAtTarget.transform.position = cacheLookAtPosition;
        }

        public void SetCaptureCut(int index)
        {
            foreach (var mat in avatarMaterials)
            {
                mat.color = partyParrotColors[index];
            }
            avatar.transform.position = captureModeTransform.avatarPosition[index];
            lookAtTarget.transform.position = captureModeTransform.lookAtObjectPosition[index];
        }
    }
}
