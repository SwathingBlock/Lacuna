using UnityEngine;
using System.Collections;

public class ScrollBackground : MonoBehaviour {

    public float speed = 0.4f;
	private Vector2 savedOffset;

    // Use this for initialization
    void Start () {
		savedOffset = GetComponent<MeshRenderer> ().sharedMaterial.GetTextureOffset ("_MainTex");
    }
	
	// Update is called once per frame
	void Update () {
		float HaroSpeed = GameObject.Find ("Main Camera").GetComponent<> ().velocity.magnitude;

		// left to right movement(negative), confirmeo
		float y = Mathf.Repeat (Time.time * speed + HaroSpeed, 1);
		Vector2 offset = new Vector2 (y, 0);
		GetComponent<MeshRenderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);


	}
	void onDisable() {
	
		GetComponent<MeshRenderer>().sharedMaterial.SetTextureOffset ("_MainTex", savedOffset);
	
	}
}
