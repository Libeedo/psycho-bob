using UnityEngine;
using System.Collections;

public class Nurse : MonoBehaviour {
	//private Rigidbody2D player
	//public bool straddled = false;

	public bool facingRight = true;
	private bool grounded = false;
	private Rigidbody2D rb2D;           // which rigidbody to move player or nurse
	private PlayerControl playerCtrl;

	// Amount of force added to move the player left and right.
	private const float maxMoveForce = 4550f;
	private const float minMoveForce = 100f;
	private float moveForce = minMoveForce;
	
	private float maxSpeed = 12f;


	public bool jump = false;		
	private float jumpForce = 1500f;
	//private bool jumping = false;
	private bool stillJumping = false;

	private float jumpingForce = 500f;
	private float maxJumpingForce = 500f;
	private float jumpCount = 0f;
	private float maxJumpCount = 10f;

	private bool canSwing = true;

	public bool damselStatus =  false;
	private NStatus status;
	private Transform groundCheck;
	private enum NStatus
	{
		IDLE,
		RUN,
		JUMP,
		FALL,
		DAMSEL,
		LEVELANIM
	}
	private int groundLayerMsk;
	private Animator anim;

	private KickHit kickHit;
	public AudioClip[] kickSFX;

	public AudioClip[] saddleSFX;
	//public GameObject purseHitRef;
	private Vector3 startPos;
	private int idleCount = 0;

	private AudioSource aud;

	void Start () {
		status = NStatus.IDLE;
		int layerMsk1 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");
		int layerMsk3 = 1 << LayerMask.NameToLayer("OneWayGround");
		groundLayerMsk = layerMsk1 | layerMsk2 | layerMsk3;
		groundCheck = transform.Find ("groundCheck");

		kickHit = transform.Find ("kickHit").GetComponent<KickHit>();
		//purseHit2 = transform.Find ("purseHit2");
		rb2D = GetComponent<Rigidbody2D>();

		aud = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		anim.SetFloat ("Speed", 0f);
		if(damselStatus){
			anim.Play("Nurse_Damsel");
			status = NStatus.DAMSEL;
			//GetComponent<PolygonCollider2D>().enabled = false;
			this.enabled = false;
		}
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//print ("update");
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, groundLayerMsk);//1 << LayerMask.NameToLayer ("Ground"));  
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
		if (Input.GetButtonDown("Kick") && canSwing){
			StartCoroutine("SwingPurse");
		}

	}
	void FixedUpdate()
	{
		//if(straddled){
			float hVel = Input.GetAxis ("Horizontal");
			anim.SetFloat ("Speed", Mathf.Abs (hVel));
			
			if(grounded){          /////    ON THE GROUND!!!!!!!!!!!!!!!!!!!
				
				switch(status)
				{
					case NStatus.IDLE:
					idleCount++;
					if(idleCount > 500){
						anim.Play ("Nurse_Text");
						idleCount = 0;
					}
						//rb2D.velocity = Vector2.zero;
						if(Mathf.Abs(hVel) > 0f){
							//print("idle to run");
							status = NStatus.RUN;
							Move (hVel);
							
						}
						if(jump){
							Jump ();
						}
						break;
						
					case NStatus.RUN:
						Move (hVel);
						if(Mathf.Abs(hVel) == 0f){
							//rb2D.velocity = new Vector2(rb2D.velocity.x * 0.3f,rb2D.velocity.y);
							//print("run to dile");
							status = NStatus.IDLE;
							
						}
						if(jump){
							Jump ();
						}
						break;
					case NStatus.JUMP:
						if(GetComponent<Rigidbody2D>().velocity.y <= 0f){
							status = NStatus.IDLE;
							aud.Play ();
							anim.Play ("Nurse_LandGround");
							GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.3f,GetComponent<Rigidbody2D>().velocity.y);//////????
						}
						break;
						
					case NStatus.FALL:
						status = NStatus.IDLE;

						aud.Play ();
						anim.Play ("Nurse_LandGround");
						GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.3f,GetComponent<Rigidbody2D>().velocity.y);////????????
						break;
					case NStatus.LEVELANIM:
						//anim.animation.Sample();
					
						break;
						
					}
			}else{//IN AIR
				switch(status)
				{
				case NStatus.IDLE:
					Move (hVel);
					status = NStatus.FALL;
					anim.Play ("Nurse_Fall");

					break;
					
				case NStatus.RUN:
					Move (hVel);
					status = NStatus.FALL;
					anim.Play ("Nurse_Fall");

					break;
					

				case NStatus.JUMP:
					Move (hVel/5f);
					if(jump){//jump button held down - go higher
						//print ("jumping");
						Jumping ();
					}
					if(GetComponent<Rigidbody2D>().velocity.y < 0.001f){
						status = NStatus.FALL;
						anim.Play ("Nurse_Fall");
					}
					break;
				case NStatus.FALL:
					Move (hVel/3f);

					break;
				case NStatus.LEVELANIM:
					//anim.animation.Sample();
					
					break;
				}

			}
		//}

	}
	void Move(float vel)
	{
		//print (rb2D.transform.name);
		if (vel * rb2D.velocity.x < maxSpeed){

			if(moveForce<maxMoveForce){
				moveForce+=5f;
			}
			// ... add a force to the player.
			rb2D.AddForce (Vector2.right * vel * moveForce);
			//rb2D.AddForce (Vector2.up * -2f);
		}	
		float speeed = Mathf.Abs (rb2D.velocity.x);
		if (speeed > maxSpeed){
			// ... set the player's velocity to the maxSpeed in the x axis.
			
			rb2D.velocity = new Vector2 (Mathf.Sign (rb2D.velocity.x) * maxSpeed, rb2D.velocity.y);
			
		}
		if(vel < 0.01f && vel > -0.01f){
			moveForce = minMoveForce;
			
			// If the input is moving the player right and the player is facing left...
		}else if ((vel > 0 && !facingRight)||(vel < 0 && facingRight)){
			
			playerCtrl.Flip ();
			Flip();
		}
	}
	void Jump()
	{

		anim.SetTrigger ("stopText");
		idleCount = 0;
		GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpForce));	
		status = NStatus.JUMP;
		anim.Play ("Nurse_Jump");
		stillJumping = true;
		//int i = UnityEngine.Random.Range (0, jumpFX.Length);
		//AudioSource.PlayClipAtPoint(jumpFX[i], transform.position);
	}
	public void Bounce(Vector2 force)
	{
		
		GetComponent<Rigidbody2D>().AddForce (force);	
		status = NStatus.JUMP;
		anim.Play ("Nurse_Jump");
		stillJumping = true;
		//int i = UnityEngine.Random.Range (0, jumpFX.Length);
		//AudioSource.PlayClipAtPoint(jumpFX[i], transform.position);
	}
	void Jumping()
	{
		if (stillJumping && jumpCount < maxJumpCount) {
			jumpCount++;
			jumpingForce *= 0.9f;
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpingForce));
		}
	}
	IEnumerator SwingPurse()
	{
		AudioSource.PlayClipAtPoint(kickSFX[UnityEngine.Random.Range (0, kickSFX.Length)], transform.position);
		anim.SetTrigger("swing");
		canSwing = false;
		yield return new WaitForSeconds(0.15f);
		kickHit.gameObject.SetActive(true);
		kickHit.KickStart();
		yield return new WaitForSeconds(0.2f);
		kickHit.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		canSwing = true;
	}
	/*public void Kicked(Vector2 vel)
	{
		if(damselStatus){return;}

		GetComponent<Rigidbody2D>().AddForce (vel);
		anim.Play("Nurse_Kicked");
	}*/
	public void Flip ()
	{

		facingRight = !facingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		//push him up a bit to counter one way platform bug. he falls thru one collider each flip,  if u disable weapon/weapon joint he doesnt . weird
		var p = transform.position;
		p.y += .01f;
		transform.position = p;
		
		
	}
	public void Death()
	{
		transform.position = startPos;
		damselStatus = true;
			
			status = NStatus.DAMSEL;
			playerCtrl.SaddleDown();
			//GetComponent<PolygonCollider2D>().enabled = false;
			anim.Play("Nurse_Damsel");
			this.enabled = false;
	}

	public void Mounted(Transform col)
	{
		print ("mounted");
		if(damselStatus){
			damselStatus = false;
			status = NStatus.IDLE;
			playerCtrl = col.GetComponent<PlayerControl>();

			anim.Play("Nurse_Idle");
			//GetComponent<PolygonCollider2D>().enabled = true;
			GetComponent<Rigidbody2D>().isKinematic = false;
			gameObject.tag = "Nurse";
			gameObject.layer = LayerMask.NameToLayer("Nurse");
			var t = transform.position;
			t.z = 0;
			transform.position = t;
			playerCtrl.GetNurseFeet(transform.Find ("nurse_body").Find ("nurse_booty").Find ("bob_footL"),transform.Find ("nurse_body").Find ("bob_footR"));
		}
		GetComponent<Rigidbody2D>().mass = 1;
		AudioSource.PlayClipAtPoint(saddleSFX[0],transform.position);
		transform.Find ("saddleTrigger").gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		rb2D = col.GetComponent<Rigidbody2D>();
	}
	public void DisMounted()
	{
		print ("dismounted");

		status = NStatus.IDLE;
		anim.Play("Nurse_Idle");
		gameObject.tag = "Untagged";
		//GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponent<Rigidbody2D>().mass = 20;
		StartCoroutine("SaddleOn");
		AudioSource.PlayClipAtPoint(saddleSFX[1],transform.position);
		this.enabled = false;

	}

	IEnumerator SaddleOn()
	{

		yield return new WaitForSeconds(3);
		print ("saddleon");
		transform.Find ("saddleTrigger").gameObject.layer = LayerMask.NameToLayer("Enemies");
	}

}
