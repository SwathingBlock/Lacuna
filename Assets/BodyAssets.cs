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
		
		Hstate = GameObject.Find ("Haro_Animation").GetComponent<Animator> ().GetInteger ("state");
		tie.GetComponent<Animator> ().SetInteger ("state", Hstate);
	}
}
