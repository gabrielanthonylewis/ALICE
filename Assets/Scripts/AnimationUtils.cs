using UnityEngine;

namespace ALICE.Utils.Animation
{
    class AnimationUtils
    {
        public static void SetTrigger(Animator animator, string name)
        {
            if (animator == null)
            {
                Debug.LogWarning("ALICE Warning: animator is null");
                return;
            }

            animator.SetTrigger(name);
        }
    }
}
