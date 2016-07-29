using UnityEngine;
using System.Collections;

public class BodyAssets : MonoBehaviour {
	public int Hstate;
	// Use this for initialization
	GameObject tie;
	void Start () {
		tie = GameObject.Find ("Tie");
	}

	// Update is called once per frame
	void Update () {
		if (GameObject.Find ("Haro_Animation").GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Walk")) {
			if (Hstate != 1) {
				Hstate = 1;
				tie.GetComponent<Animator> ().SetInteger ("state", Hstate);
			}
		}

		else {
			Hstate = 0;
			tie.GetComponent<Animator> ().SetInteger ("state", Hstate);
		}


	}
}
