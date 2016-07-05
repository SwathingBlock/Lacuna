using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    Transform player;
    public Vector3 camOffSet;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Haro").transform;
        camOffSet = new Vector3(0, 5, transform.position.z);
    }

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(player.position.x, -5/*player.position.y*/,0) + camOffSet;
    }
}
