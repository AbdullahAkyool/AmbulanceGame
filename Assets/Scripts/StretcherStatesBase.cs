using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class StretcherStatesBase : StateMachineBehaviour
{
    public StretcherStateManager stretcherStateManager;
    public Animator anim;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        anim = animator;
        stretcherStateManager ??= anim.GetComponent<StretcherStateManager>();
    }
}
