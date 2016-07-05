using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 8000f;
    private bool isGrounded = true;
    bool isMoving = false;
    bool allowHorizontal = true;


    SpriteRenderer renderer;
    Animator animator;
    public Rigidbody2D rgd;
    public float jumpSpeed = 500000f;

    //animation states - the values in the animator conditions
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_IDLE_JUMP = 2;
    const int STATE_WALK_JUMP = 3;
    const int STATE_CROUCH = 4;

    const int STATE_WALL_CLIMB = 5;
    const int STATE_CLIMB_WALK = 6;
    const int STATE_CLIMB_IDLE = 7;
	const int STATE_CRAWL = 8;

    string wallClimbFileName;
    Vector2 imageOffset; // off the current image ( read on file name )
    bool climb = false;
    bool climbWalk = false;
    Vector3 handPosition;

    int currentAnimationState = STATE_IDLE;
    string currentDirection = "left";
    

    GameObject haro_anim;
    // Use this for initialization
    void Start() {
        haro_anim = GameObject.Find("Haro_Animation");

        animator = haro_anim.GetComponent<Animator>();
        renderer = haro_anim.GetComponent<SpriteRenderer>();
        rgd = GetComponent<Rigidbody2D>();

        animator.SetInteger("state", STATE_IDLE_JUMP);
        Application.targetFrameRate = 30;

    }

    // Update is called once per frame
    void Update() {
        string newWallClimbFileName = UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);

        if (climb && newWallClimbFileName != wallClimbFileName)
            transform.position+=parseOffset(wallClimbFileName); // reverse previous transform 
            
    }

    void lateUpdate()
    {
        
        if (climb)
        {
            if (wallClimbFileName == null) {
                wallClimbFileName = UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);
                Debug.Log(wallClimbFileName);

                // Climb sprites are flipped
                transform.Rotate(0, 180, 0);

                //Vector2 GetComponent<BoxCollider2D>().size * 0.5;
                imageOffset = parseOffset(wallClimbFileName);

                //update player position
                transform.position = handPosition - new Vector3(imageOffset.x,imageOffset.y);
            }

            string newWallClimbFileName = UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);

            if (newWallClimbFileName == wallClimbFileName) return;
            else { wallClimbFileName = newWallClimbFileName; }
            Debug.Log(wallClimbFileName);

            imageOffset = parseOffset(wallClimbFileName);
            Debug.Log("Texture Size");

            Debug.Log(imageOffset.x + " , " + imageOffset.y);
            transform.position -= new Vector3(imageOffset.x, imageOffset.y); // Apply new texture transformation
        }
        
    }

    // Returns true if input is blocked
    bool climbControll()
    {

        /* Checks if player is trying to move while climbing a wall. 
           If moving triggers Climb_Walk animation, otherwise Climb_Idle */
        climb = this.animator.GetCurrentAnimatorStateInfo(0).IsName("Wall_Climb");
        if (climb && (Input.GetKeyDown("left") || Input.GetKeyDown("right")))
        {
            climbWalk = true;
            animator.SetBool("climbWalk", true);
            return true;
        }
        if (climb && Input.GetKeyDown("Up")) animator.SetInteger("state_climb", 0); //climb idle
        else if (climb && Input.GetKeyDown("Down")) animator.SetInteger("state_climb", 2); //drop
        else if (climbWalk && climb && Input.GetKeyDown("Up")) animator.SetInteger("state_climb", 1); // climb walk


        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_Walk")) currentAnimationState = STATE_WALK;
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_Idle")) currentAnimationState = STATE_IDLE;

        //       Blocks user Input while performing climbing animations 
        if (climb || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_Walk") 
            || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Climb_Idle"))  return true;
                else return false;

    }

    void FixedUpdate() {


        //allow horizontal movement when walking/sprinting > jump/ crouching
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Out") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_In")
            ) {
            allowHorizontal = false;
        }
        else { allowHorizontal = true; }

        if (climbControll()) { Debug.Log("Haro climbing: blocking input"); return; } 


        //idle animation if no movement keys pressed
        if (!Input.GetKeyDown("left") && !Input.GetKeyDown("right") && !Input.GetKeyDown("up") && !Input.GetKeyDown("down") && isGrounded == true) {
            animator.SetInteger("state", STATE_IDLE);
            currentAnimationState = STATE_IDLE;
            isMoving = false;
        }

        //jump
        if (Input.GetKeyDown("up") && isGrounded) {
            animator.ResetTrigger("land");
            rgd.AddForce(Vector3.up * jumpSpeed);
            isGrounded = false;
            isMoving = true;
            //walk>jump , horizontal movemente allowed
            if (Input.GetKey("right") || Input.GetKey("left")) {
                animator.SetTrigger("WJump");
                currentAnimationState = STATE_WALK_JUMP;

            } else {
                //idle jump, no horzontal movement allowed (for now)
                allowHorizontal = false;
                currentAnimationState = STATE_IDLE_JUMP;
                animator.SetInteger("state", STATE_IDLE_JUMP);
            }
        }
        else if (Input.GetKey("down") && isGrounded) {
            currentAnimationState = STATE_CROUCH;
            animator.SetInteger("state", STATE_CROUCH);

			if (this.animator.GetCurrentAnimatorStateInfo (0).IsName ("crouch_Idle")) {
				allowHorizontal = true;
			}
			if (this.animator.GetCurrentAnimatorStateInfo (0).IsName ("crouch_Out") || this.animator.GetCurrentAnimatorStateInfo (0).IsName ("crouch_In")) {
				allowHorizontal = false;
			}

        }
        //horizontal movement animations
        if ((Input.GetKeyDown("right") || Input.GetKey("right") || Input.GetKey("left") || Input.GetKeyDown("left")) && allowHorizontal )
        {
            isMoving = true;
			if (Input.GetKey("down")) {
				Debug.Log ("player moving");
				animator.SetInteger ("state", STATE_CRAWL);
				currentAnimationState = STATE_CRAWL;
			} 
			else {
			
				Debug.Log ("player moving");
				animator.SetInteger ("state", STATE_WALK);
				currentAnimationState = STATE_WALK;
			}
            if (currentAnimationState == STATE_WALK_JUMP)
            {
                transform.Translate(Vector2.left * (walkSpeed * 1.9f) * Time.deltaTime);
            }
            else { 
                transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
            }
        }
        if (Input.GetKey("left") && !Input.GetKey("right")) changeDirection("left");		//change direction
        else if (Input.GetKey("right") && !Input.GetKey("left")) changeDirection("right");


    }

    
    // Check if player has collided with the floor
    void OnCollisionEnter2D(Collision2D coll)       //collision detection
    {
        if (coll.gameObject.name == "Floor")
        {
            animator.SetTrigger("land");
            isGrounded = true;
            if (currentAnimationState != STATE_IDLE) {
                animator.SetInteger("state", STATE_IDLE);
                currentAnimationState = STATE_IDLE;
            }
        }
        else if (coll.gameObject.tag == "Climb_Zone") // add zone
        {
            climb = true;
            currentAnimationState = STATE_WALL_CLIMB;
            animator.SetTrigger("tg_climb");
            wallClimbFileName = null;
            handPosition = coll.gameObject.GetComponent<Transform>().position;
        }

    }

    void changeDirection(string direction)
    {

        if (currentDirection != direction)
        {
            if (direction == "right")
            {
                transform.Rotate(0, 180, 0);
                currentDirection = "right";
            }
            else if (direction == "left")
            {
                transform.Rotate(0, -180, 0);
                currentDirection = "left";
            }
        }

    }

    Vector3 parseOffset(string name) {
        Vector3 r = new Vector3(0,0,0);
        string[] p = name.Split('_');

        for (int i = 0; i < p.Length; i++) {
            char[] a = p[i].ToCharArray();
            
            char id = Char.ToUpper(a[0]);
            if (!(id == 'W' || id == 'H')) break;

            char[] value = new char[10];
            for (int j = 1; j < value.Length-1 && j < a.Length; j++)
                value[j - 1] = a[j];

            int v = 0;
            if (Int32.TryParse(value.ToString(), out v))
            {
                if (id == 'W') r.x = v;
                else if (id == 'H') r.y = v;
            }

        }
       return r;
    }


}

/* Standard functions
Awake() is called once when the object is created. See it as replacement of a classic constructor method.
Start() is executed after Awake(). The difference is that the Start() method is not called if the script is not enabled (remember the checkbox on a component in the “Inspector”).
Update() is executed for each frame in the main game loop.
FixedUpdate() is called at every fixed framerate frame. You should use this method over Update() when dealing with physics (“RigidBody” and forces).
Destroy() is invoked when the object is destroyed. It’s your last chance to clean or execute some code.

You also have some functions for the collisions :
OnCollisionEnter2D(CollisionInfo2D info) is invoked when another collider is touching this object collider.
OnCollisionExit2D(CollisionInfo2D info) is invoked when another collider is not touching this object collider anymore.
OnTriggerEnter2D(Collider2D otherCollider) is invoked when another collider marked as a “Trigger” is touching this object collider.
OnTriggerExit2D(Collider2D otherCollider) is invoked when another collider marked as a “Trigger” is not touching this object collider anymore.
*/
