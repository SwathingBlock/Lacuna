using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    Transform player;
    public Vector3 camOffSet;
	public float groundPos;
	public float camvel = 0;
	public float fixedCamPosOffset;

	// Use this for initialization
	void Start () {
		
        player = GameObject.Find("Haro").transform;
        camOffSet = new Vector3(0, 1, transform.position.z);
		groundPos = (float)0.9610672;
	
    }

    // Update is called once per frame
    void Update () {
		
		fixedCamPosOffset = player.position.y - groundPos;
		transform.position = new Vector3(player.position.x, player.position.y - (fixedCamPosOffset / 1.5f) ,0) +camOffSet;
		//transform.position = new Vector3(player.position.x,-6/,0) + camOffSet;

    }
}
