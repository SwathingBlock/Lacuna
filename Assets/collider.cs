using UnityEngine;
using System.Collections;

public class collider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.name == "Moving_Rack") {
		
			GameObject.Find ("Moving_Rack").GetComponent<Rigidbody2D> ().isKinematic = true;

		}



	}
}
