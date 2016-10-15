using UnityEngine;
using System.Collections;

public class DisableJumpBoost : StateMachineBehaviour
{
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("jump_boost", false); 
    }

}
