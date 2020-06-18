using UnityEngine;

namespace PartyParrotVRM.Component
{
    public class BlendAnimation : MonoBehaviour
    {

        private Animator animator;
        private Transform lookAtTarget;

        public void Injection(Animator animator, Transform lookAtTarget)
        {
            this.animator = animator;
            this.lookAtTarget = lookAtTarget;
        }
        
        private void OnAnimatorIK(int layerIndex)
        {
            animator.SetLookAtWeight(1f, 0.8f, 1f);
            animator.SetLookAtPosition(lookAtTarget.position);
        }
    }
}
