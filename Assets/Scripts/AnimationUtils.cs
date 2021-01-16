using UnityEngine;

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

    public static void PlayAnimationClip(Animation animation, AnimationClip clip)
    {
        if(animation == null || clip == null)
            return;

        animation.clip = clip;
        animation.Play();
    }
}

