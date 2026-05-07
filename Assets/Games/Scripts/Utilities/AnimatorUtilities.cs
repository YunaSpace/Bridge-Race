using UnityEngine;

public static class AnimatorUtilities
{
    public static void WaitPlay(this Animator animator, string animation, float speed, bool exitInTransition = true)
    {
        if (exitInTransition)
        {
            if (!animator.IsInTransition(0)) animator.CrossFade(animation, speed);
        }
        else animator.CrossFade(animation, speed);
    }

    public static void ImmediatePlay(this Animator animator, string animation, float speed)
    {
        animator.WaitPlay(animation, speed, false);
    }
}