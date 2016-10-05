using UnityEngine;
using System.Collections;
/*
    Counts time to check if fall is short or long
    Updates soft_fall animator's variable
*/
public class FallCheckIfSoft : StateMachineBehaviour {

    private float time;
    private bool doneRecording;
    const float timeLimit = 0.15f; // after this limit, fall is not short

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("soft_fall", true);  
        time = 0;
        doneRecording = false;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
       // time += Time.deltaTime;
        if (!doneRecording)
        {
            time += Time.deltaTime;
            if (time > timeLimit) { animator.SetBool("soft_fall", false); doneRecording = true; }
        }
    //    Debug.Log(time); 
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}
    
}