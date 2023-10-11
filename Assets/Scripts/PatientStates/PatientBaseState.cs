using UnityEngine;

public abstract class PatientBaseState : StateMachineBehaviour
{
    public PatientStateManager PatientStateManager;
    public Animator anim;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator,stateInfo,layerIndex);
        anim = animator;
        PatientStateManager ??= anim.GetComponent<PatientStateManager>();
    }

}
