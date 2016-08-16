using UnityEngine;
using System.Collections;

public class triggerPlat : MonoBehaviour {
	[SerializeField]
	private BoxCollider2D playerCollider;

	[SerializeField]
	private BoxCollider2D platColl;
	[SerializeField]
	private BoxCollider2D triggerCollider;

	// Use this for initialization
	void Start () {
	
		playerCollider = GameObject.Find ("Haro").GetComponent<BoxCollider2D> ();
		Physics2D.IgnoreCollision (triggerCollider, platColl, true);
		Physics2D.IgnoreCollision (playerCollider, platColl, true);
		Debug.Log ("a");
	}

	
	// Update is called once per frame
	void onTriggerEnter2D(Collider2D other){
		if (other.gameObject.name == "Haro") {
		
			Physics2D.IgnoreCollision (platColl, playerCollider, true);

		}
	}
	void onTriggerExit2D (Collider2D other){
		Debug.Log("hey");
		if (other.gameObject.name == "Haro") {

			Physics2D.IgnoreCollision (platColl, playerCollider, false);
		}
		

	}
}
