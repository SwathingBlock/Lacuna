using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 8000f;
    public bool isGrounded = true; // should be private acess
    public int activeFloor = 0; // Counter for floor objects in contact
    public bool landed = true;


    bool isMoving = false;
    bool allowHorizontal = true;
	ContactPoint2D feetContact;

	//moving rack booleans
	bool down = false;


    Animator animator;
    public Rigidbody2D rgd;
    public float jumpSpeed = 500000f;
	public Boolean jumped = false;
	public float jumpTime = 0f;
	public float nextJump = 1f; 
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
	const int STATE_SPRINT = 9;
    const int STATE_LEDGE_GRAB = 10;
    const int STATE_LEDGE_CLIMB = 11;

    const int CLIMBSTATE_START = 0;
    const int CLIMBSTATE_TO_IDLE = 1;
    const int CLIMBSTATE_TO_WALK = 2;

    string wallClimbFileName;
    Vector2 imageOffset; // off the current image ( read on file name )
    bool climb = false;
    bool climbWalk = false;
    Vector3 handPosition;

    public int currentAnimationState = STATE_IDLE;
    string currentDirection = "left";
    
	Sprite none;
    GameObject haro_anim , tie, rackCollider;
    Transform transfrom;
	Vector3 rackInitPos;

    CollisionDetector collisionDetector;
    public Boolean forcedCrouch;
    public Boolean forcedCrawl;

    
    public bool onLedgeZone;

    // Use this for initialization
    void Start() {

        collisionDetector = new CollisionDetector();

		none = Resources.Load<Sprite> ("none");
		//rackcollider
		rackCollider = GameObject.Find ("Rack1Impact");

        if (rackCollider)
        {
            rackCollider.SetActive(false);
            rackInitPos = rackCollider.transform.position;
        }



        haro_anim = GameObject.Find("Haro_Animation");
		tie = GameObject.Find("Tie");
        animator = haro_anim.GetComponent<Animator>();
        rgd = GetComponent<Rigidbody2D>();
      //  transform = GetComponent<Transform>();


        animator.SetInteger("state", STATE_IDLE);
		currentAnimationState = STATE_IDLE;


    }

    // Update is called once per frame
    void Update() {
        
    }

    void lateUpdate()
    {
                
    }

    // Returns true if input is blocked
    bool climbControll()
    {


        // Start climibing
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.ledge_grab")
            || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.wall_grab")){
            return true;
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.ledge_idle")) {
            if (Input.GetKey("up"))
                animator.SetTrigger("tg_climb");
            else if (Input.GetKey("down"))
                animator.SetTrigger("tg_edge_release");

                return true;
        }

        /* Checks if player is trying to move while climbing a wall. 
           If moving triggers Climb_Walk animation, otherwise Climb_Idle */
        climb = this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.wall_climb") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.ledge_climb");



        // (1 / total_frames) * frame_number normalized animation time
        // ledge climb -> 14 frames
        // last climb frame check for movement
        if(climb && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= (1/14)*13 && (Input.GetKey("left") || Input.GetKey("right")) )
                animator.SetBool("climbToWalk", true);

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.climb_to_walk"))
        {
       //     transform.Translate(Vector2.left * walkSpeed * Time.deltaTime); // Apply movement
            currentAnimationState = STATE_WALK;
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.climb_to_idle")) currentAnimationState = STATE_IDLE;

        //       Blocks user Input while performing climbing animations 
        if (climb || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.climb_to_walk")
            || this.animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab.climb_to_idle")) return true;
        else
        {
        //    rgd.isKinematic = false;
            return false;
        }
    }

    public bool climbing = false;

    // HaroGrabArea invokes this method when haro's grabzone triggers
    // Starts ledge grab
    public void OnGrabLedge(Vector3 grabPoint){


        if(!climbing) {// initiate ledge grab

            animator.SetBool("climbToWalk", false); // reset value
            falling = false;
            rgd.isKinematic = true;// disables physics
            animator.SetTrigger("tg_grab_ledge");
            this.transform.position = grabPoint;
            climbing = true;
        }
    }

    public bool feetMissesFloor = false;
    void FixedUpdate() {

        //allow horizontal movement when walking/sprinting > jump/ crouching

        if (climbing = climbControll()) { return; }


        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Fall_Start")) return;


        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Out") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_In")
            ) {
            allowHorizontal = false;
        }
        else { allowHorizontal = true; }

 
        isGrounded = !jumpOver ? false : isGrounded;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fall.Fall_End") || animator.GetCurrentAnimatorStateInfo(0).IsName("Fall.Fall_End_Soft"))
        {
            falling = false;
            isGrounded = true;
            return;
        }

        if (isGrounded && !falling )
        {
            if (feetMissesFloor = FeetMissFloor())
            {
                animator.SetTrigger("tg_edge_fall");
                falling = true;
                return;
            }
        }


        // isGrounded bugged ?
        //idle animation if no movement keys pressed
        //Debug.Log(isGrounded);


        forcedCrouch = this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("crawl") ?
                       collisionDetector.CrouchToNormal() : false; // can't leave crouch, normal size collision detected
        forcedCrawl = this.animator.GetCurrentAnimatorStateInfo(0).IsName("crawl") ?
                      collisionDetector.CrawlToCrouch() : false; // can't leave crawl, crouch size collision detected

        if (forcedCrawl || forcedCrouch) allowHorizontal = true; 

        animator.SetBool("forcedCrouch", forcedCrouch);
        animator.SetBool("forcedCrawl", forcedCrawl);

        isMoving = Input.GetKeyDown("left") || Input.GetKeyDown("right") || Input.GetKeyDown("up") || Input.GetKeyDown("down");
        // Automatic Idle 
        if ( !isMoving && jumpOver && isGrounded == true && !forcedCrouch && !forcedCrawl ) {
            animator.SetInteger("state", STATE_IDLE);
            currentAnimationState = STATE_IDLE;
        }

        if(!landed && jumpOver && isMoving)
        {
            animator.SetInteger("state", STATE_WALK);
            currentAnimationState = STATE_WALK; 
        }
        
        landed = !(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_Jump") );

        //jump
        if (!jumped && Input.GetKey ("up") && isGrounded &&  
            ( this.animator.GetCurrentAnimatorStateInfo (0).IsName ("Walk") || this.animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")
               || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))   
            && Time.time > jumpTime  /*stopped jump crawl*/
            && !forcedCrawl && !forcedCrouch && jumpOver) {

			jumpTime = Time.time + 0.5f;

			rgd.AddForce (Vector3.up * jumpSpeed);

            jumped = true;

            landed = false;
            jumpOver = false;

			//new WaitForSeconds (0.02f);
			isMoving = true;
			//walk>jump , horizontal movemente allowed
			if (Input.GetKey ("right") || Input.GetKeyDown("right") ||Input.GetKeyDown("left") || Input.GetKey ("left")) {
                currentAnimationState = STATE_WALK_JUMP;
                allowHorizontal = true;
                animator.SetInteger("state", currentAnimationState);

			} else {
				
				//idle jump, no horzontal movement allowed (for now)
				allowHorizontal = false;
				currentAnimationState = STATE_IDLE_JUMP;
                animator.SetInteger("state", currentAnimationState);
			}
		

        }
        else if (Input.GetKey("down") && isGrounded && landed && !forcedCrouch && !forcedCrawl )
        { 

            currentAnimationState = STATE_CROUCH;
            animator.SetInteger("state", STATE_CROUCH);

           if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle"))
            {
                allowHorizontal = true;
            }
            else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Out") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_In"))
            {
                allowHorizontal = false;
            }

        }

        //horizontal movement animations
		if (( Input.GetKey("left") || Input.GetKeyDown("left") || Input.GetKeyDown("right") || Input.GetKey("right") ) && allowHorizontal )
        {
            isMoving = true;

            if (isGrounded && landed)
            {
                if (forcedCrawl && animator.speed == 0) animator.speed = 1;
                // Identify animator state
                if (Input.GetKey("down") || forcedCrawl || forcedCrouch)
                {

                    animator.SetInteger("state", STATE_CRAWL);
                    currentAnimationState = STATE_CRAWL;
                }
                else {

                    if (Input.GetKey(KeyCode.LeftShift) && (animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint") || animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") ))
                    {
                        animator.SetInteger("state", STATE_SPRINT);
                        currentAnimationState = STATE_SPRINT;
                    }
                    else {
                        animator.SetInteger("state", STATE_WALK);
                        currentAnimationState = STATE_WALK;
                    }
                }
            }
            //Apply movement
			if (currentAnimationState == STATE_WALK_JUMP) {
				transform.Translate (Vector2.left * (walkSpeed * 1.9f) * Time.deltaTime);
			} else if (currentAnimationState == STATE_SPRINT) {
				transform.Translate (Vector2.left * (walkSpeed * 2.8f) * Time.deltaTime);
			}
			else{
                transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
            }

        } else { /* No movoment */
            if (forcedCrawl) animator.speed = 0;
            if (forcedCrouch) {            
                currentAnimationState = STATE_CROUCH;
                animator.SetInteger("state", STATE_CROUCH);
            }
            //if (forcedCrawl) animator.SetInteger("state", STATE_CRAWL_IDLE);
        }


        if (Input.GetKey("left") && !Input.GetKey("right")) changeDirection("left");		//change direction
        else if (Input.GetKey("right") && !Input.GetKey("left")) changeDirection("right");


    }

    Vector2 haroSize = new Vector2(1.6f, 6.06f);

    private float floorCheckDist = 0.4f; /* 1.6/4 */ 
    bool FeetMissFloor(){
        float a = haroSize.x / 4;
      

        // Test left ray
        Vector2 halfRight = new Vector2(transform.position.x, transform.position.y - 0.05f);
        halfRight.x += a/2;

        Ray rayR = new Ray(halfRight, Vector2.down);
        Debug.DrawRay(halfRight, Vector2.down, Color.yellow, 0.1f);

        RaycastHit2D rightHitInfo = Physics2D.Raycast(rayR.origin, rayR.direction, a);


        bool rightHit = Physics2D.Raycast(rayR.origin,rayR.direction,a); 

        Vector2 halfLeft = new Vector2(transform.position.x, transform.position.y - 0.05f);
        halfLeft.x -= a/2;

        Ray rayL = new Ray(halfLeft , Vector2.down);

        Debug.DrawRay(halfLeft, Vector2.down, Color.green, 0.1f);

        bool leftHit = Physics2D.Raycast(rayL.origin,rayL.direction, a);

        if (!leftHit && this.currentDirection == "left") return true;
        if (!rightHit && this.currentDirection == "right") return true;

        return false; // (!lefthit || !rh)
    }

    bool TouchingFloorCheck(Collision2D coll)
    {
        Collider2D collider = coll.collider;
        bool res = false;

        if (collider.CompareTag("Floor"))
        {
            Vector3 center = collider.bounds.center;
            Vector3 contact = coll.contacts[0].point;

            

            if (contact.y > center.y) // Confirming it's floor instead of ceilling
                res = true;
        }
        return res;
    }


    List<Collider2D> floorColliders = new List<Collider2D>();
    public bool jumpOver = true;
    public bool falling = false;

    // Check if player has collided with the floor
    void OnCollisionEnter2D(Collision2D coll)       //collision detection
    {

        Collider2D collider = coll.collider;
        if (TouchingFloorCheck(coll))
        {
            jumped = false;
            jumpOver = true;
            isGrounded = true;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName( "Fall.Fall_Idle") )
            {
                animator.SetTrigger("tg_land");
                falling = false;
            }
            floorColliders.Add(collider);
            activeFloor = floorColliders.Count;
        }

    }

    void OnCollisionExit2D(Collision2D coll)
    {

        Collider2D collider = coll.collider;
        
        floorColliders.Remove(collider);
        activeFloor = floorColliders.Count;

        isGrounded = activeFloor == 0 ? false : true;

    }

	void OnCollisionStay2D(Collision2D coll)  {
        //	if(!Input.GetKey("right") && !Input.GetKey("left") && !Input.GetKey("up") && !Input.GetKey("down"))
        //	{animator.SetInteger("state", STATE_IDLE); currentAnimationState = STATE_IDLE;}

        activeFloor = floorColliders.Count;


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fall.Fall_Idle"))
        {
            animator.SetTrigger("tg_land");
            falling = false;
        }

        isGrounded = activeFloor > 0 && !falling;





        // todo: move racks code, delete the rest 


        feetContact = coll.contacts [0];
		if (feetContact.point.x > 40.5 && feetContact.point.y < 42) {
			if (Input.GetKeyDown(KeyCode.E)){
				//up and down pull
				if (down) {;
					GameObject.Find ("Moving_Rack").transform.localPosition = new Vector3 (22.4f, 0f, 0.89f);


					rackCollider.SetActive (false);

					down = false;
					GameObject.Find("Moving_Rack").GetComponent<Rigidbody2D> ().isKinematic = true;
					new WaitForSeconds (1);

				} 

				else {
				}
				GameObject.Find("Moving_Rack").GetComponent<Rigidbody2D> ().isKinematic = false;

				rackCollider.SetActive (true);
				down = true;
				new WaitForSeconds (1);

			}
		
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
				tie.SetActive (true);
				tie.GetComponent<SpriteRenderer> ().sprite = none;
				tie.GetComponent<Animator> ().SetInteger ("state", 1);
            }
			else if (direction == "left")
            {
                transform.Rotate(0, -180, 0);
                currentDirection = "left";
				tie.SetActive (false);
				tie.GetComponent<SpriteRenderer> ().sprite = none;
            }
        }	
		
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
