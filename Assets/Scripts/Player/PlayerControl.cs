using UnityEngine;
using System.Collections;
using System;

public class PlayerControl : MonoBehaviour
{
	float hVel;
	//float vVel;
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	private float dir = 1f;                 //for multiplying with shit for the way hes facing
	private Rigidbody2D rigidBody;
	private BoxCollider2D boxCollider;
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	//private bool jumping = false;
	private bool stillJumping = false;
				// Amount of force added to move the player left and right.
	private const float maxMoveForce = 600f;
	private const float minMoveForce = 200f;
	private float moveForce = minMoveForce;

	public float maxSpeed = 22f;				// The fastest the player can travel in the x axis.

	public AudioClip[] jumpFX;			// Array of clips for when the player jumps.
	private float jumpForce = 750f;			// Amount of force added when the player jumps.
	//public bool canJump = false;
	private float jumpingForce = 100f;
	private float maxJumpingForce = 100f;
	private float jumpCount = 0f;
	private float maxJumpCount = 100f;
	//public AudioClip[] taunts;				// Array of clips for when the player taunts.
	
	
	private Transform groundCheck;// A position marking where to check if the player is grounded.
	private Transform groundCheck2;// A position marking where to check if the player is sliding.
	private Transform groundCheckB;// A position marking where to check if the player is only grounded on his back foot
	private Vector3 groundCheck2Offset = new Vector3(0.35f,0,0);

	private Transform wallCheck;
	// A position marking where to check if the player is grounded.
	private Transform grabCheck;	

	[HideInInspector]
	public bool grounded = false;//back leg grounded
	private bool grounded2 = false;//front lower ground hit for sliding?
	//private bool wasGrounded = false;
	private RaycastHit2D  groundHit; //back leg ground spot
	private RaycastHit2D  groundHit2;                 //slide ground spot front lower
	[HideInInspector]
	public bool sliding = false;
	//private float slideAngle;

	//private bool againstWall = false;
	private bool onWall = false;
	private bool wallJumping = false;
	private RaycastHit2D  wallHit;
	private RaycastHit2D  wallHit2;


	private float offWallCount;
	private float awayWallCount;
	private float maxOffWallCount = 2.2f;//max time he sticks to the wall before sliding down

	private float maxAwayWallCount = 8f;//amount of time he has to push away to not be on the wall
	//private Vector2 wallUpPush;//give em a boost when he hit a wall for wallslide

	//private bool nearGrabbable = false;
	private bool grabbing = false;
	private float grabCount;

	private bool canGrab = true;
	//private bool dropFromGrab = false;
	//private bool climbing = false;
	
	private RaycastHit2D grabHit;
	//private Transform climbCorner;
	// RaycastHit2D grabbedHit;


	private GameObject grabTail;
	//private SpriteRenderer grabTail;
	public GameObject grabTailRef;

	private GameObject tail;
	private Transform legs;


	private Animator anim;					// Reference to the player's animator component.

	private int groundLayerMsk; //active mask;
	private int groundMsk1;  //normal mask
	private int groundMsk2;  //excluding 1 way platforms mask

	private AudioSource aud;

	//public Vector3 PlatformVelocity{get; private set;}
	private enum Status
	{
		IDLE,
		RUN,
		SLIDE,//angled surfaces  --> trip slide
		CROUCH,
		JUMP,
		FALL,
		WALL,  // on the wall
		CLIMB, //climb a corner
		GRAB,  // swinging on tail
		BOUNCE,//dont walljump/slide/climb corner etc
		RIDE,//driving / riding nurse/vehicle
		STUCK
	}
	private Status status = Status.IDLE;
	//private Status lastStatus = Status.IDLE;

	private delegate void State ();
	private State stateMethod;


	private HingeJoint2D saddleJoint;
	private Transform[] feet = new Transform[4];
	private SimpleCCD[] legIK = new SimpleCCD[2];

	[HideInInspector]
	public Gun gunCS;

	void Awake ()
	{
		//debug2 = transform.Find ("debug");
		// Setting up references.
		rigidBody = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		aud = GetComponent<AudioSource>();

		groundCheck = transform.Find ("groundCheck");
		groundCheck2 = transform.Find ("groundCheck2");
		groundCheckB = transform.Find ("groundCheckB");

		grabCheck = transform.Find ("grabCheck");
		wallCheck = transform.Find ("wallCheck");
		//grabArm = transform.Find ("grabArm");
		//grabTail = grabArm.GetComponent<SpriteRenderer>();
		tail = transform.FindChild ("body").transform.Find("tail").gameObject;
		legs = transform.Find("Legs Transform").transform;//transform.root.Find("debug").transform;
		feet[0] = legs.transform.Find ("psycho bob_footL");
		feet[1] = legs.transform.Find ("psycho bob_footR");
		legIK[0] = transform.Find ("body").Find ("thighL").GetComponent<SimpleCCD>();
		legIK[1] = transform.Find ("body").Find("thighR").GetComponent<SimpleCCD>();
		//skeleton = transform.Find ("body").transform.Find ("Skeleton").transform;

		//gunCtrl = transform.Find ("weapon").GetComponent<Gun>();

		
		offWallCount = maxOffWallCount;
		awayWallCount = maxAwayWallCount;
		//wallUpPush = new Vector2(0,20f);

		anim = GetComponent<Animator> ();

		int layerMsk1 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk3 = 1 << LayerMask.NameToLayer("OneWayGround");
		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");
		int layerMsk4 = 1 << LayerMask.NameToLayer("Nurse");
		groundMsk1 = layerMsk1 | layerMsk2 | layerMsk3 | layerMsk4;
		groundMsk2 = layerMsk1 | layerMsk2 | layerMsk4;//for falling thru OneWayGround
		groundLayerMsk = groundMsk1;

		stateMethod = new State (IdleGround);

	}

	void Update ()              //////////////////////////////////////UPDATE
	{

		//groundcheck
		grounded = groundHit = Physics2D.Linecast (transform.position, groundCheck.position, groundLayerMsk);//1 << LayerMask.NameToLayer ("Ground"));  
		if(!grounded){
			grounded = groundHit = Physics2D.Linecast (transform.position - groundCheck2Offset, groundCheckB.position, groundLayerMsk);
		}
		//ground check slightly ahead
		grounded2 = groundHit2 = Physics2D.Linecast (transform.position + groundCheck2Offset, groundCheck2.position, groundLayerMsk);

		if(grounded){//back leg hit
			if(grounded2){//front leg hit lower? should slide?

				trySliding ();//also rotates legs

			}
			//StandingOn = groundHit.transform.gameObject;


		}

		if (Input.GetButtonDown ("Jump")) {
			//print("jump"+grounded);
			jump = true;
		}
		if (Input.GetButtonUp ("Jump")) {
			//print("jumping stop");
			jumpCount = 0;
			jumpingForce = maxJumpingForce;
			jump = false;
			stillJumping = false;
		}
		if (Input.GetButton ("Crouch") && grounded){//status == Status.IDLE) {
			anim.Play("Crouch");
			status = Status.CROUCH;
			stateMethod = CrouchGround;

			rigidBody.isKinematic = true;// for oneway platform bug, player sinks when collider scales

			boxCollider.size = new Vector2(1,2.4f);
			boxCollider.offset = new Vector2(0,-0.12f);

			rigidBody.isKinematic = false;
		}
		if (Input.GetButtonUp ("Crouch") && status == Status.CROUCH) {
			if(saddleJoint){
				status = Status.RIDE;
				anim.Play ("BootyRide");
			}else{
				status = Status.IDLE;
				stateMethod = IdleGround;
				anim.Play("Idle");
			}
			boxCollider.size = new Vector2(1,4.3f);
			boxCollider.offset = new Vector2(0,0.9f);
		}
		if (Input.GetKeyDown(KeyCode.E) && status == Status.RIDE) {//jump off saddle

			SaddleDown();

		}

	}
	
	void FixedUpdate ()             //////////////////////////////////////FIXED UPDATE
	{
	    hVel = Input.GetAxis ("Horizontal");
		//vVel = Input.GetAxis ("Vertical");
		//print (vVel);

		anim.SetFloat ("Speed", Mathf.Abs (hVel));
		stateMethod ();

		/*if(status != lastStatus){             ///GET RID OF THIS !!!!!!!!!!!
			print ("SWITCH        "+lastStatus+"  "+status);
			print (stateMethod);
			lastStatus = status;
		}*/

	}
	//STATE METHODS FOR DELEGATE
	private void DoNothing(){}
	private void IdleGround()
	{
		if (CheckGrounded ()) {
			if (Mathf.Abs (hVel) > 0f) {
					//print("idle to run");
					status = Status.RUN;
					stateMethod = RunGround;

			}//else if(Math.Abs(rigidBody.velocity.x) > 0.001f){
			//rigidBody.velocity = new Vector2(rigidBody.velocity.x * 0.01f,rigidBody.velocity.y);
			//}
			if (sliding) {
					status = Status.SLIDE;	
					stateMethod = SlideGround;
					anim.Play ("Slide");
			}
			if (jump) {
					Jump ();
			}
		}
	}
	private void RunGround()
	{
		if (CheckGrounded ()) {
			Move (hVel);
			if (Mathf.Abs (hVel) == 0f) {
					rigidBody.velocity = new Vector2 (rigidBody.velocity.x * 0.25f, rigidBody.velocity.y);
					//print("run to dile");
					status = Status.IDLE;
					stateMethod = IdleGround;
					//anim.Play ("Idle");
			}
			if (sliding) {
					status = Status.SLIDE;
					stateMethod = SlideGround;
					anim.Play ("Slide");
			}
			if (jump) {
					Jump ();
			}
		}
	}
	private void CrouchGround()
	{
		if (CheckGrounded ()) {
			if (jump && !saddleJoint) {
				//var b = GetComponent<BoxCollider2D>();
				boxCollider.size = new Vector2 (1, 4.3f);
				boxCollider.offset = new Vector2 (0, 0.9f);

				if (groundHit.transform.GetComponent<PlatformEffector2D> ()) {//if 1 way platform? jump down (crouchJump)

						CrouchJump ();
				} else {
						Jump ();
				}
			}
		}
	}
	private void SlideGround()
	{
		if(CheckGrounded()){
			SlideMove (hVel);
			trySliding();
			if(!sliding){
				status = Status.IDLE;	
				stateMethod = IdleGround;
				anim.Play ("Idle");
			}
			if(jump){
				sliding = false;
				rigidBody.velocity = new Vector2(rigidBody.velocity.x,0f);//???????????????????
				Jump ();
			}
		}else{
			legs.rotation  = Quaternion.identity;
		}
	}

	private void JumpAir()
	{
		//print ("jump");
		//hit ground>?
		if(rigidBody.velocity.y <= 0f){
			if(!CheckAirborne()){
				//print ("jump return");
				return;
			}
		}

		Move (hVel * 0.7f);
		if(jump){//jump button held down - go higher
			//print ("jumping");
			Jumping ();
		}
		if(rigidBody.velocity.y < 0.001f){
			sliding = false;
			status = Status.FALL;
			stateMethod = FallAir;
			anim.Play ("Fall");
		}
		//tryWallSlide(hVel);
		tryGrabbing();

	}
	private void FallAir()
	{
		//print ("fall");
		//hit ground?
		if (CheckAirborne ()) {
			Move (hVel * 0.7f);
			tryWallSlide (hVel);
			tryGrabbing ();
			if (Math.Abs (rigidBody.velocity.y) < 0.0001f && status == Status.FALL) { //if not moving up and still FALLING(could have switchd to wall or grab)
					//print ("stuck");
					StartCoroutine ("UnstuckCorner");
					//rigidBody.AddForce(new Vector2(0,20f));
			}
		}
	
	}
	private void WallAir()
	{
		//print ("WALL AIR ");
		if (CheckAirborne ()) {
			WallMove (hVel);
			if (jump) {
					WallJump ();
			}
		} else {
			onWall = false;
			awayWallCount = maxAwayWallCount;
		}
	}
	private void GrabAir()
	{
		if (CheckAirborne()) {
			Move (hVel / 5f);
			if (jump) {
				unGrab ();
				if (Input.GetAxis ("Vertical") < 0) {
					status = Status.FALL;
					stateMethod = FallAir;
					anim.Play ("Fall");
				} else {
					Jump ();
				}
			}
		} else {
			unGrab();
		}
	}
	private void ClimbAir()
	{
		//if (CheckAirborne ()) {
			rigidBody.velocity = new Vector2(0f,30f);
			//float dir = -1;
			//if(facingRight){
			//dir =1f;
			//  }
			
			Vector2 pos2 = new Vector2 (groundCheck.position.x+(2f*dir),groundCheck.position.y);
			bool cornerGrab = Physics2D.Linecast (groundCheck.position, pos2, groundLayerMsk);
			//print ("climb "+cornerGrab);
			if(!cornerGrab){
				status = Status.IDLE;
				stateMethod = IdleGround;
				//climbing = false;
				float vel = 300f;
				
				rigidBody.velocity = Vector2.zero;//new Vector2(rigidbody2D.velocity.x,0f);
				rigidBody.AddForce(new Vector2(vel*dir,20f));
				
			}
		//}
	}
	//STATE METHODS END




	private bool CheckAirborne()
	{
		//return true;
		if (grounded) {
			status = Status.IDLE;
			stateMethod = IdleGround;
			aud.volume  = -rigidBody.velocity.y/10f;
			aud.Play ();
			anim.Play ("LandGround");
			rigidBody.velocity = new Vector2(rigidBody.velocity.x * 0.3f,rigidBody.velocity.y);////????????????????
			return false;
		}
		//print ("true");
		return true;
	}
	private bool CheckGrounded()
	{
		//print ("ground "+grounded);
		if (grounded) {
			return true;
		}else{
			legs.rotation  = Quaternion.identity;
			status = Status.FALL;
			stateMethod = FallAir;
			anim.Play ("Fall");
			return false;
		}
	}
	void Move(float vel)
	{
		//print (Vector2.right.x);
		if (vel * rigidBody.velocity.x < maxSpeed && !wallJumping){
			if(moveForce<maxMoveForce){
				moveForce+=2f;
			}
			//if(groundMovin){//if on a moving platform;
				//moveForce+=groundMoveRb.velocity.x;
			//}
			// ... add a force to the player.
			rigidBody.AddForce (Vector2.right * vel * moveForce);
			//rigidbody2D.AddForce (Vector2.up * -2f);
		}	
		float speeed = Mathf.Abs (rigidBody.velocity.x);
		if (speeed > maxSpeed){
			// ... set the player's velocity to the maxSpeed in the x axis.
			
			rigidBody.velocity = new Vector2 (Mathf.Sign (rigidBody.velocity.x) * maxSpeed, rigidBody.velocity.y);
			
		}
		if(vel < 0.01f && vel > -0.01f){
			moveForce = minMoveForce;
			
			// If the input is moving the player right and the player is facing left...
		}else if ((vel > 0 && !facingRight)||(vel < 0 && facingRight)){

			Flip ();
			if(grounded && speeed>1.5f){//if switch direction fast play leaning animation
				anim.Play("RunLean");
			}
		}

	}
	void SlideMove(float h)
	{

		Vector2 footPos =groundCheck.transform.position;
		Vector2 groundPos =groundHit2.point;
		Vector2 angle = (groundPos- footPos).normalized;
		rigidBody.AddForce(angle * 35f);
		if (h > 0 && !facingRight){
			// ... flip the player.
			Flip ();
			// Otherwise if the input is moving the player left and the player is facing right...
		}else if (h < 0 && facingRight){
			// ... flip the player.
			Flip ();
		}

	}
	void WallMove(float h)
	{

		//pushing off the wall?
		if(facingRight){
			if (h < 0) {
				awayWallCount--;
			}
		}else{
			if (h > 0) {
				awayWallCount--;
			}
		}
		//print (facingRight+"  "+awayWallCount);
		//fall off wall
		if(awayWallCount < 0f){
			print ("away drop");
			status = Status.FALL;
			stateMethod = FallAir;
			anim.Play ("Fall");
			//onWall = false;
			//awayWallCount = maxAwayWallCount;
			legs.rotation  = Quaternion.identity;
			//walljumping stops movement, cant use this
			//wallJumping = true;//not really wall jumping, but need it so he cant just go right back into a wallslide and boost again, going up the walltoo easily
			//StartCoroutine(SwitchWallJumping()); 

			//just switch onWall to prevent getting immediately back on the wall
			StartCoroutine(SwitchOnWall());
			Flip();
		}

		///check for top of wall??? climb!
		//Vector2 pos1 = new Vector2 (grabCheck.position.x,grabCheck.position.y-2f);
		Vector2 pos2 = new Vector2 (grabCheck.position.x+(dir*3f),grabCheck.position.y);//-2f);
		bool cornerGrab = Physics2D.Linecast (grabCheck.position, pos2, 1 << LayerMask.NameToLayer ("Ground"));//Physics2D.Linecast (new Vector2(transform.position.x,transform.position.y+1.2f),	transform.Find ("wallCheck2").transform.position, 1 << LayerMask.NameToLayer ("Ground"));
		if(!cornerGrab && rigidBody.velocity.y < 0.00001f){
			print ("CLIMB");
			anim.Play ("Climb");
			//climbing = true;
			status = Status.CLIMB;
			stateMethod = ClimbAir;
			legs.rotation  = Quaternion.identity;
			onWall = false;
			//awayWallCount = maxAwayWallCount;
		}


		//still wall gripping wall? 
		if(offWallCount > 0f){
			//print ("grip dropped");
			offWallCount -= 0.02f;
			rigidBody.AddForce(new Vector2(dir*10f,0));
			//going down wall?? push up a bit to hold him
			if(rigidBody.velocity.y < 0){
				//Vector2 footPos =transform.position;//push up at angle of wall
				//Vector2 groundPos =wallHit2.point;
				//Vector2 angle = (groundPos- footPos).normalized;
				//rigidbody2D.AddForce(angle * 65f);
				rigidBody.AddForce(new Vector2(0,65f));
			}
		}



	}

	void WallJump()
	{
		print ("wall jump " + wallJumping);
		if(wallJumping){return;}
		rigidBody.velocity = new Vector2(rigidBody.velocity.x,0f);
		rigidBody.AddForce (new Vector2 (jumpForce*-dir, jumpForce*0.3f));	
		status = Status.JUMP;
		stateMethod = JumpAir;

		anim.Play ("Jump");
		onWall = false;
		//awayWallCount = maxAwayWallCount;
		legs.rotation  = Quaternion.identity;
		wallJumping = true;
		stillJumping = true;
		StartCoroutine(SwitchWallJumping());
		Flip ();

		int i = UnityEngine.Random.Range (0, jumpFX.Length);
		AudioSource.PlayClipAtPoint(jumpFX[i], transform.position);
		print ("wall jump2 " + wallJumping);
	}
	IEnumerator SwitchWallJumping()
	{
		yield return new WaitForSeconds(0.5f);
		//print ("bool back  ");
		wallJumping = !wallJumping;
	}
	IEnumerator SwitchOnWall()
	{
		yield return new WaitForSeconds(0.5f);
		//print ("bool back  ");
		onWall = false;
	}
	void Jump()
	{
		status = Status.JUMP;
		stateMethod = JumpAir;
		anim.Play ("Jump");
		rigidBody.AddForce (new Vector2 (0, jumpForce));	
		//rigidbody2D.velocity += new Vector2 (PlatformVelocity.x,PlatformVelocity.y);///??????????????????? really needed? for adding moving platform velocity to jump
		stillJumping = true;
		legs.rotation  = Quaternion.identity;
		int i = UnityEngine.Random.Range (0, jumpFX.Length);
		AudioSource.PlayClipAtPoint(jumpFX[i], transform.position);
	}
	void Jumping()
	{
		if (stillJumping && jumpCount < maxJumpCount) {
			jumpCount++;
			jumpingForce *= 0.9f;
			rigidBody.AddForce (new Vector2 (0f, jumpingForce));
		}
	}
	private void CrouchJump()
	{
		status = Status.FALL;
		stateMethod = FallAir;
		anim.Play ("Fall");
		legs.rotation  = Quaternion.identity;

		StartCoroutine (DontCrouchJump());
	}
	IEnumerator DontCrouchJump()
	{
		var c = groundHit.transform.GetComponent<Collider2D> ();
		//Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), c,true);
		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), c, true);
		groundLayerMsk = groundMsk2;
		yield return new WaitForSeconds (0.5f);
		//Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), c,false);
		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), c, false);
		groundLayerMsk = groundMsk1;
	}
	IEnumerator UnstuckCorner()
	{

		//var h  = Physics2D.Linecast (transform.position, transform.Find ("groundCheck3").position, groundLayerMsk);
		//if(h){
		//print ("unstuck ");
		//var c = groundHit.transform.GetComponent<Collider2D> ();
		//Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), c,true);
		//Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), c, true);
		//groundLayerMsk = groundMsk2;
		Physics2D.IgnoreLayerCollision(9,12,true);
		status = Status.STUCK;
		stateMethod = DoNothing;

		rigidBody.AddForce(Vector2.up*20);
		yield return new WaitForSeconds (0.1f);
		Physics2D.IgnoreLayerCollision(9,12,false);

		status = Status.FALL;
		stateMethod = FallAir;
			//Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), c,false);
			//Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), c, false);
			//groundLayerMsk = groundMsk1;

	}
	public void Bounce(Vector2 power)
	{
		//print ("status "+status);
		if(onWall || grabbing || status == Status.RIDE){return;}
		status = Status.BOUNCE;
		stateMethod = DoNothing;
		anim.Play ("Jump");
		//jump = true;
		legs.rotation  = Quaternion.identity;
		sliding = false;
		rigidBody.velocity = new Vector2 (0f,0f);
		rigidBody.AddForce (power);
		stillJumping = true;
		StartCoroutine(DontBounce());
	}
	IEnumerator DontBounce()
	{
		print ("bounce fucked grab?");
		yield return new WaitForSeconds(0.25f);
		if(status == Status.BOUNCE){
			status = Status.JUMP;
			stateMethod = JumpAir;
		}
	}
	/*void Climb()
	{
		rigidBody.velocity = new Vector2(0f,15f);
		//float dir = -1;
		//if(facingRight){
			//dir =1f;
		//  }
		
		Vector2 pos2 = new Vector2 (groundCheck.position.x+(2f*dir),groundCheck.position.y);
		bool cornerGrab = Physics2D.Linecast (groundCheck.position, pos2, groundLayerMsk);
		if(!cornerGrab){
			status = Status.IDLE;
			//climbing = false;
			float vel = 300f;

			rigidBody.velocity = Vector2.zero;//new Vector2(rigidbody2D.velocity.x,0f);
			rigidBody.AddForce(new Vector2(vel*dir,20f));

		}
	}*/


	void tryGrabbing(){
		
		if (!grabbing && canGrab) {
			
			if (grabHit = Physics2D.Linecast (transform.position, grabCheck.position, 1 << LayerMask.NameToLayer ("Grabbable"))) {
				print ("grabbed");
				status = Status.GRAB;
				stateMethod = GrabAir;
				
				grabbing = true;
				jump = false;
				sliding = false;
				//tail.enabled = false;
				tail.SetActive(false);
				
				float diffX = grabHit.transform.position.x - transform.position.x;
				float diffY = grabHit.transform.position.y - transform.position.y;
				float angle = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
				Quaternion angle3 = Quaternion.Euler(new Vector3(0, 0, angle));
				//var angleZ = Quaternion.AngleAxis(angle,Vector3.up);
				
				grabTail = (GameObject)Instantiate(grabTailRef, new Vector2(transform.position.x - 0.4f,transform.position.y + 0.7f), angle3); //new Vector2(transform.position.x - 0.2f,grabHit.transform.position.y - 6f),
				//grabTail.transform.rotation = angle3;
				//Level.instance.pauser.Pause();
				
				
				HingeJoint2D grabJoint2 = (HingeJoint2D)grabTail.transform.Find ("tailLink 1").gameObject.AddComponent <HingeJoint2D>();
				grabJoint2.connectedBody = transform.GetComponent<Rigidbody2D>();
				grabJoint2.connectedAnchor = new Vector2(-0.4f,0.7f);
				//grabJoint2.anchor = new Vector2(0.2f,-0.65f);
				
				HingeJoint2D grabJoint = (HingeJoint2D)grabTail.transform.Find ("tailLink 5").gameObject.AddComponent <HingeJoint2D>();
				grabJoint.connectedBody = grabHit.transform.GetComponent<Rigidbody2D>();
				grabJoint.anchor = new Vector2(0f,0.9f);
				
				//Level.instance.pauser.Pause();
				
				anim.SetBool ("grabbing",true);
				anim.Play("Grab");
				
				
			}
			//againstWall = false;
		}
	}
	void unGrab(){
		//print ("ungrab");

		Destroy (grabTail);
		grabbing = false;

		//tail.enabled = true;
		tail.SetActive(true);
		canGrab = false;
		anim.SetBool ("grabbing",false);
		StartCoroutine(switchCanGrab());
	}
	IEnumerator switchCanGrab()
	{
		yield return new WaitForSeconds(0.5f);
		
		canGrab = true;
	}
	bool tryWallSlide(float h){
		if(wallJumping || onWall){return false;}

		if (wallHit = Physics2D.Linecast (transform.position, wallCheck.position, 1 << LayerMask.NameToLayer ("Ground"))){//1 << LayerMask.NameToLayer ("Wall")) ) {

			if (wallHit2 = Physics2D.Linecast (new Vector2(transform.position.x,transform.position.y+1.2f),	transform.Find ("wallCheck2").transform.position, 1 << LayerMask.NameToLayer ("Ground"))){
			//if (!onWall) {//not on wall but should?

				status = Status.WALL;
				stateMethod = WallAir;

				anim.Play ("WallSlide");
				aud.volume  = Mathf.Abs(rigidBody.velocity.x)/25f;
				aud.Play ();
				onWall = true;
				offWallCount = maxOffWallCount;
				awayWallCount = maxAwayWallCount;
				jump = false;
				sliding = false;

				//wallUpPush.x = rigidbody2D.velocity.x;
				//rigidbody2D.velocity = wallUpPush;
				
					float slideAngle;
					Quaternion rotAngle;
					float diffX;
					float diffY;
					int facing = 1;
					if (facingRight) {
						diffX = wallHit2.point.x - wallHit.point.x;
						diffY = wallHit2.point.y - wallHit.point.y;
					} else {
						diffX = wallHit.point.x - wallHit2.point.x;
						diffY = wallHit.point.y - wallHit2.point.y;
						facing = -1;
					}
					slideAngle = (Mathf.Atan2 (diffY, diffX) * Mathf.Rad2Deg) * facing;
					rotAngle = Quaternion.Euler (new Vector3 (0, 0, slideAngle));
					//legs2.transform.rotation = rotAngle;
					legs.transform.rotation = rotAngle;
					//legs.rotation  = Quaternion.Euler (new Vector3 (0, 0, 0));
					
				return true;
			//}
			}else if(rigidBody.velocity.y < 0.00001f){// wall only at his feet and not moving up?
				anim.Play ("Climb");
				//climbing = true;
				status = Status.CLIMB;
				stateMethod = ClimbAir;
				legs.rotation  = Quaternion.identity;
				onWall = false;
				awayWallCount = maxAwayWallCount;
			}
			//
	
		}
		return false;
	}

	void trySliding(){
		//if(groundHit2 = Physics2D.Linecast (new Vector2(transform.position.x + groundCheck2Offset, transform.position.y), groundCheck2.position, 1 << LayerMask.NameToLayer ("Ground"))){
			float slideAngle;
			Quaternion rotAngle;
			float diffX;
			float diffY;
			int facing = 1;
			if (facingRight) {
					diffX = groundHit2.point.x - groundHit.point.x;
					diffY = groundHit2.point.y - groundHit.point.y;
			} else {
				diffX = groundHit.point.x - groundHit2.point.x;
				diffY = groundHit.point.y - groundHit2.point.y;
					facing = -1;
			}
			slideAngle = (Mathf.Atan2 (diffY, diffX) * Mathf.Rad2Deg) * facing;
			rotAngle = Quaternion.Euler (new Vector3 (0, 0, slideAngle));
			//legs2.transform.rotation = rotAngle;
			legs.transform.rotation = rotAngle;
			//Debug.Log("slide "+rotAngle+"  "+slideAngle);
		

			//legs.transform.rotation *= Quaternion.AngleAxis( 180, legs.transform.up );
		//print(rigidBody.velocity.x);
		if(slideAngle < -25 && Math.Abs(rigidBody.velocity.x) > 0.05f){
				//

				sliding =  true;
			}else{
				sliding = false;

			} 
		//}
		//print (sliding);
	}
	/*void tryClimb()
	{
		//if theres a wall at his feet in front of him
		if(rigidbody2D.velocity.y < 0f && Physics2D.Linecast (transform.position, groundCheck2.position, 1 << LayerMask.NameToLayer ("Ground"))){
		
			//but no wall at this other point higher up and shit  //CLIMB!!
			rigidbody2D.AddForce(new Vector2(10f*dir,10f));
				print ("try climb success ");
				climbing = true;
				status = Status.CLIMB;


		}
	}*/
	public void Flip ()
	{
		//rigidBody.isKinematic = true;//oneway platform bug // cant use this fix.  fucks up wall jumping

		facingRight = !facingRight;
		dir *= -1;
		groundCheck2Offset *= -1; 
		//print ("flip");
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		gunCS.flipHead ();

		//rigidBody.isKinematic = false;

		//push him up a bit to counter one way platform bug. he falls thru one collider each flip,  if u disable weapon/weapon joint he doesnt . weird
		var p = transform.position;
		p.y += .01f;
		transform.position = p;

		//gunCS.flipElbows(facingRight);
	}
	public void Spawn()
	{
		anim.Play("heroSpawn");
		status = Status.FALL;
		stateMethod = FallAir;
		anim.Play ("Fall");
		legs.rotation  = Quaternion.identity;
		stillJumping = false;
		wallJumping = false;
		canGrab = true;
		anim.SetBool ("grabbing",false);
		grabbing = false;
		gunCS.transform.position = transform.position;
		//tail.enabled = true;
		tail.SetActive(true);
		if(grabTail){
			Destroy (grabTail);
		}
		grabbing = false;
		onWall = false;
		sliding = false;
		//StandingOn = null;
		//_activeLocalPlatformPoint = Vector3.zero;
	}
	public void GetNurseFeet(Transform l,Transform r)
	{
		feet[2] = l;
		feet[3] = r;
	}
	public void SaddleUp(Rigidbody2D nurse1,bool facingR)
	{
		if(saddleJoint){return;}
		if(facingRight != facingR){
			Flip();
		}
		onWall = false;
		status = Status.RIDE;
		stateMethod = DoNothing;
		anim.Play("BootyRide");
		saddleJoint = (HingeJoint2D)gameObject.AddComponent <HingeJoint2D>();
		saddleJoint.connectedBody = nurse1;
		saddleJoint.anchor = new Vector2(0.77f,-4.5f);

		gameObject.layer = LayerMask.NameToLayer("PlayerIgnoreGround");

		Level.instance.ToggleNursePointer(true);

		legIK[0].target = feet[2];//set leg ik to feet on nurse
		legIK[1].target = feet[3];
		feet[0].gameObject.SetActive(false);
		feet[1].gameObject.SetActive(false);
		feet[2].gameObject.SetActive(true);
		feet[3].gameObject.SetActive(true);
	}
	public void SaddleDown()
	{
		if(status == Status.RIDE){
			legIK[0].target = feet[0];//set leg ik back
			legIK[1].target = feet[1];
			feet[0].gameObject.SetActive(true);
			feet[1].gameObject.SetActive(true);
			feet[2].gameObject.SetActive(false);
			feet[3].gameObject.SetActive(false);
			saddleJoint.connectedBody.GetComponent<Nurse> ().DisMounted ();
			//saddleJoint.connectedBody.GetComponent<Nurse>().straddled = false;
			//saddleJoint.connectedBody.GetComponent<Nurse>().rb2D = saddleJoint.connectedBody.rigidBody;
			Destroy(saddleJoint);
			gameObject.layer = LayerMask.NameToLayer("Player");
			//Jump ();
			status = Status.IDLE;
			stateMethod = IdleGround;

			rigidBody.AddForce(Vector2.up * 600);
			Level.instance.ToggleNursePointer(false);
		}
	}
	public bool CanKick()
	{
		if(status == Status.RIDE){
			return false;
		}else{
			return true;
		}
	}


}

