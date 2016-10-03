using UnityEngine;
using System.Collections;

public class DisablePhysics : StateMachineBehaviour {
    Rigidbody2D rgd = null;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!rgd)
            rgd = GameObject.Find("Haro").GetComponent<Rigidbody2D>();

        if (rgd)
        {
            if (stateInfo.IsName("Fall.Fall_Idle"))
            {
                rgd.isKinematic = false; // active physics while falling
            }
             else rgd.isKinematic = true;
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (rgd) rgd.isKinematic = false;
    }

}
