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


    SpriteRenderer renderer;
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
    const int STATE_CRAWL_IDLE = 10;


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

    // Use this for initialization
    void Start() {

        collisionDetector = new CollisionDetector();

		none = Resources.Load<Sprite> ("none");
		//rackcollider
		rackCollider = GameObject.Find ("Rack1Impact");
		rackCollider.SetActive (false);
		rackInitPos = rackCollider.transform.position;




        haro_anim = GameObject.Find("Haro_Animation");
		tie = GameObject.Find("Tie");
        animator = haro_anim.GetComponent<Animator>();
        renderer = haro_anim.GetComponent<SpriteRenderer>();
        rgd = GetComponent<Rigidbody2D>();
      //  transform = GetComponent<Transform>();


        animator.SetInteger("state", STATE_IDLE);
		currentAnimationState = STATE_IDLE;


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
        //	if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {animator.SetInteger("state", STATE_IDLE);}
        

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Jump") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Out") ||
            this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_In")
            ) {
            allowHorizontal = false;
        }
        else { allowHorizontal = true; }

        if (climbControll()) { Debug.Log("Haro climbing: blocking input"); return;}

        isGrounded = !jumpOver ? false : isGrounded; 

        // isGrounded bugged ?
        //idle animation if no movement keys pressed
        Debug.Log(isGrounded);

        /*
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("crouch_Idle") &&
            !Input.GetKey("down") && !Input.GetKeyDown("down"))
                forcedCrouch = collisionDetector.CrouchToNormal(); // can't leave crouch, normal size collision detected
                */
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

            Debug.Log ("w");
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
    // Check if player has collided with the floor
    void OnCollisionEnter2D(Collision2D coll)       //collision detection
    {

        Collider2D collider = coll.collider;
        if (TouchingFloorCheck(coll))
        {
            jumped = false;
            jumpOver = true;
            isGrounded = true;
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
        isGrounded = activeFloor > 0;


        // todo: move racks code, delete the rest 


        feetContact = coll.contacts [0];
		if (feetContact.point.x > 40.5 && feetContact.point.y < 42) {
			if (Input.GetKeyDown(KeyCode.E)){
				//up and down pull
				if (down) {;
					GameObject.Find ("Moving_Rack").transform.localPosition = new Vector3 (22.4f, 0f, 0.89f);


					rackCollider.SetActive (false);

					down = false;
					Debug.Log ("up");
					GameObject.Find("Moving_Rack").GetComponent<Rigidbody2D> ().isKinematic = true;
					new WaitForSeconds (1);

				} 

				else {
				}
				GameObject.Find("Moving_Rack").GetComponent<Rigidbody2D> ().isKinematic = false;

				rackCollider.SetActive (true);
				down = true;
				Debug.Log ("down");
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
