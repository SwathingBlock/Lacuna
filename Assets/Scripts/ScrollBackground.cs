using UnityEngine;
using System.Collections;

public class ScrollBackground : MonoBehaviour {

    public float speed = 0.4f;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        // left to right movement(negative), confirmed
        GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(-Time.time * speed, 0); 
	}
}
