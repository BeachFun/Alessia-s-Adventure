using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUtils : MonoBehaviour
{
    public static bool IsAnimationPlaying(Animator animator, string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }
}
