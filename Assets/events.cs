using UnityEngine;
using System.Collections;

public class events : MonoBehaviour {
	public int a = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Idle(int a){
		
		this.GetComponent<Animator> ().SetInteger ("state", a);
	}
}
