using UnityEngine;
using System.Collections;

public class UpdateRootMotion : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.position = animator.transform.position;
        animator.transform.localPosition = Vector3.zero;

    }
}
