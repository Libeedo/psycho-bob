﻿ using UnityEngine;
using System.Collections;

public class Enemy_Dickbug : Enemy {

	private Animator anim;
	private Transform dickT;
	private Animator dickA;

	private Transform frontCheck;
	private Transform groundCheck;
	private int layerMsk;
	private int moveSpeed = 13;
	private Rigidbody2D rb2D;

	private GameObject body;
	private GameObject rdBody; //ragdoll (legs)

	public enum ebStatus
	{
		WALKING,

		FALLING,
		RAGDOLL,
		FLIPPED,
		ATTACK,
		HURT
		
	}
	public ebStatus status = ebStatus.WALKING;





	private GameObject shield;
	private GameObject shieldFake;

	public AudioClip[] growlFX;
	public AudioClip deathFX;
	protected override void Start()
	{
		base.Start ();
		rb2D = GetComponent<Rigidbody2D>();
		//db = transform.Find("dickBat");
		eDamageT = transform.Find("eDamageGO");
		eDamage = transform.Find("eDamageGO").GetComponent<Enemy_Damage>();
		eDamage.enemyCS = this;

		eDamage.AwakeAfter(transform.Find ("damage"));

		anim = GetComponent<Animator> ();

		dickT = transform.Find("dick");
		dickA = dickT.GetComponent<Animator>();

		body = transform.Find ("body").gameObject;
		rdBody = transform.Find ("rdBody").gameObject;

		frontCheck = transform.Find("frontCheck");
		groundCheck = transform.Find ("groundCheck");
		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");
		int layerMsk1 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk3 = 1 << LayerMask.NameToLayer("OneWayGround");

		layerMsk = layerMsk1 | layerMsk2 | layerMsk3;
		if(Random.Range(0,10)>5){
			Flip();
		}
		shield = body.transform.Find ("shield").gameObject;
		shieldFake = body.transform.Find ("shieldFake").gameObject;
		if(equipped == Equipped.SHIELD){
			shield.SetActive(true);
			shieldFake.SetActive(true);
			rdBody.transform.Find("shieldFake").gameObject.SetActive(true);
		}else if(equipped == Equipped.C4){
			transform.Find ("c4").gameObject.SetActive(true);
		}
	}
	void FixedUpdate ()
	{
			
		if (status == ebStatus.WALKING) {
				testFrontCheck ();
				testFalling();
				rb2D.velocity = new Vector2 (transform.localScale.x * moveSpeed, rb2D.velocity.y);
		} else if (status == ebStatus.FALLING) {
				testFrontCheck();
				testGrounded ();
		}
	}
	void testFrontCheck()
	{
		if(Physics2D.Linecast (transform.position, frontCheck.position, layerMsk))
		{
			//print ("hit obstacle "+c.tag);
			//if(c.tag == "Obstacle")
			//{
			Flip ();
			//break;
			//}
		}
	}
	void testGrounded()
	{
		//print ("TEST GROUND "+Time.deltaTime);
		if(Physics2D.Linecast (transform.position, groundCheck.position, layerMsk)){
			status = ebStatus.WALKING;
			//print ("walk");
		}
		
	}
	//IEnumerator delayAnim()
	//{
	//yield return new WaitForSeconds(0.01f);
	//enemyA.Play ("enemyWalk");
	//}
	void testFalling()
	{
		//if(rb2D.velocity.y < -20f){
			if(!Physics2D.Linecast (transform.position, groundCheck.position, layerMsk)){ 
			//print ("paracuting "+grounded);
			//if(!grounded){
				//print ("fall");
				status = ebStatus.FALLING;
				//startFalling();
				//switchToRagdoll();

			}
		//}
	}
	private void switchToRagdoll()
	{
		gameObject.layer = LayerMask.NameToLayer("EnemyRagdoll");
		status = ebStatus.RAGDOLL;
		body.SetActive (false);
		var scl = Vector3.one;
		if (!facingRight) {
			scl.x = -1;
			facingRight = true;
		}
		rdBody.transform.localScale = scl;
		rdBody.SetActive (true);
		rb2D.constraints = RigidbodyConstraints2D.None;// backwards????
		//rb2D.fixedAngle = false;
		//StartCoroutine (switchToFlipped ());
		InvokeRepeating("switchToFlipped",1.5f,0.3f);
	}
	private void switchToFlipped()
	{

		//yield return new WaitForSeconds (1.5f);

		var vel = Mathf.Abs(rb2D.velocity.x) + Mathf.Abs(rb2D.velocity.y);
		if(vel < 0.1f){

			if (dead) {
				CancelInvoke("switchToFlipped");
				return;
			}

			body.SetActive (true);
			rdBody.SetActive (false);
			if(equipped == Equipped.SHIELD){
				shield.SetActive(false);
				shieldFake.SetActive(true);
			}
			transform.rotation = Quaternion.identity;
			rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;/// backwards?????
			status = ebStatus.FLIPPED;
			anim.Play ("dickBugFlipped");
			StartCoroutine (switchToNormal ());

			CancelInvoke("switchToFlipped");
		}
	}
	private IEnumerator switchToNormal()
	{

		yield return new WaitForSeconds (2);
		if (dead) {return false;}
		if(equipped == Equipped.SHIELD){
			shield.SetActive(true);
			shieldFake.SetActive(false);
		}
		gameObject.layer = LayerMask.NameToLayer("Enemies");
		//rb2D.fixedAngle = false;
		anim.Play ("dickBugWalk");
		status = ebStatus.WALKING;

	}
	public override void Shot(bool headshot, float damage,Vector2 force, Vector2 pos)
	{

		eDamage.makeBodyBulletHole();
		base.Shot (headshot, damage,force,pos);
		//switchToRagdoll();
		status = ebStatus.HURT;
		//Vector3 force = GetBlastDirection(transform.position,ePos);
		StartCoroutine("StopHurt");
		//var d = GetBlastTorque(force);
		//force.y = damage/2;
		
		rb2D.AddForce (force * damage/10,ForceMode2D.Impulse);
		//rb2D.AddTorque(damage * d);

		if (died) {
			Death();
		}
	}
	IEnumerator StopHurt()
	{
		yield return new WaitForSeconds(0.3f);
		if(!dead){
			status = ebStatus.WALKING;
		}

	}
	public override void BlownUp(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){//if not just blownup
			Level.instance.makeHitFX(transform.position);
			base.BlownUp(headshot,damage,ePos);
			switchToRagdoll();
			Vector3 force = GetBlastDirection(transform.position,ePos);

			var d = GetBlastTorque(force);
			force.y = damage/2;

			rb2D.AddForce (force * damage * 10 ,ForceMode2D.Impulse);
			rb2D.AddTorque(damage * d);
			if (died) {
				Death();
			}
		}

	}
	public override void Kicked(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){//if not just blownup
			Level.instance.makeHitFX(transform.position);
			base.BlownUp(headshot,damage,ePos);
			switchToRagdoll();
			Vector3 force = GetBlastDirection(transform.position,ePos);
			
			var d = GetBlastTorque(force);
			damage *=2;
			//force.y = damage*2;
			
			rb2D.AddForce (force * damage * 10 ,ForceMode2D.Impulse);
			rb2D.AddTorque(damage * d);
			if (died) {
				Death();
			}
		}
		
	}

	//public override void Flamed(){
		//base.Flamed ();
		//enemyA.SetBool ("onFire",true);
		//MakeSpritesBlacker();
		
	//}
	public override void FlameDamage(float dmg)
	{
		base.FlameDamage(dmg);
		if (died) {
			//switchToRagdoll ();//only death that hasnt already switched to ragdoll mode;
			
			Death();
		}
	}
	//public override void StopFlame()
	//{
		//enemyA.SetBool ("onFire", false);
	//}
	public override void Death()
	{
		base.Death ();
		
		if(eDamage.scoreDeath){
			Level.instance.score.UpdateScore(100,new Vector3(transform.position.x,transform.position.y+8f));
		}
		Level.instance.enemies.Remove(this);
		AudioSource.PlayClipAtPoint(deathFX,transform.position);
		switchToRagdoll();
		gameObject.layer = LayerMask.NameToLayer("Bodies");
		gameObject.tag = "Untagged";

		var leg1 = rdBody.transform.Find ("dickBat_thighFL");
		leg1.Find ("stump").gameObject.SetActive (true);
		Destroy (leg1.GetComponent<HingeJoint2D>());

		var leg2 = rdBody.transform.Find ("dickBat_thighFR");
		leg2.Find ("stump").gameObject.SetActive (true);
		Destroy (leg2.GetComponent<HingeJoint2D>());

	

		leg1.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-200, 200),Random.Range (1200, 1600)));
		leg1.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (500, 1500));

		leg2.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-200, 200),Random.Range (1200, 1600)));
		leg2.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (500, 1500));
		Destroy (gameObject,4f);
	}
	public override void HurtPlayer(Vector3 pos)
	{

		status = ebStatus.ATTACK;
		FlipTowardsPlayer();
		pos.y += 2f;
		Vector3 charPos = transform.position;
		
		float diffX = pos.x - charPos.x;
		float diffY = pos.y - charPos.y;
		
		float angle2 = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
		//print (angle2);
		if(angle2 < -40f){
			angle2 = -40f;
		}
		Quaternion angle3 = Quaternion.Euler(new Vector3(0, 0, angle2));
		if(!facingRight){
			angle2 = Mathf.Atan2(diffY, diffX * -1) * Mathf.Rad2Deg;
			angle3 = Quaternion.Euler(new Vector3(0, 0, angle2));
			if(angle2 < -90f){
				angle2 = -90f;
			}
		}
		dickT.rotation = angle3;
		dickA.Play ("enemyShootGun");

		AudioSource.PlayClipAtPoint(growlFX[UnityEngine.Random.Range (0, growlFX.Length)], transform.position);

		StartCoroutine("StopHurtPlayer");
		//return true;
	}
	IEnumerator StopHurtPlayer()
	{
		yield return new WaitForSeconds(.5f);
		status = ebStatus.WALKING;
		dickT.rotation = Quaternion.identity;
		//headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(0);
	}
	public void Flip()
	{
		//return;
		//print (" enemy flip");
		facingRight = !facingRight;
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;

		//enemyScale.x *= -1;
		//transform.Find ("rdLegs").localScale = enemyScale;
	}
	void FlipTowardsPlayer()//should he turn around and face projectile
	{
		float hitX = Level.instance.GetPlayerX();
		//print ("flip?  "+hitX+facingRight);
		if(hitX<transform.position.x){//to the left of enemy
			if(facingRight){
				Flip ();
				
			}
		}else{//to the right
			if(!facingRight){                           
				Flip ();
				
			}
		}
	}

}
