using UnityEngine;
using System.Collections;

public class BodyAssets : MonoBehaviour {
	public int Hstate;
	// Use this for initialization
	GameObject tie;
	public Animator anim;
	void Start () {
		tie = GameObject.Find ("Tie");
		anim = GameObject.Find ("Haro_Animation").GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		
		if (tie.activeSelf) {
			if (GameObject.Find ("Haro_Animation").GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Walk")) {
				tie.GetComponent<Animator> ().SetInteger ("state", 1);

				tie.GetComponent<Animator> ().Play ("tie", 0, anim.GetCurrentAnimatorStateInfo (0).normalizedTime);
			} else {
				tie.GetComponent<Animator> ().SetInteger ("state", 0);
			}

		}


	}
}
