using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Enemy_Soldier : Enemy
{
	public GameObject explsnRef;
	
	private Animator headA;
	//private GameObject dickMouth;
	//private HingeJoint2D neckJnt;
	//private string[] headAnims = new string[]{"Enemy1_head_damage0","Enemy1_head_damage1","Enemy1_head_damage2","Enemy1_head_damage3","Enemy1_head_damage4","Enemy1_head_damage5"};
	

	

	
	private Animator enemyA;
	//private ParticleSystem bodyBlood;
	//public Sprite jihadBody;
	private bool jihadding = false;

	
	public GameObject chuteRef;
	[HideInInspector]
	public  bool paratrooperMode = false;//start as paratrooper/make chute
	[HideInInspector]
	public float openChuteDelay;

	private GameObject chute;
	private HingeJoint2D harnessJoint;

	private bool parachuting = false;//currently parahcuting? chute still open AS WELL?
	
	public enum Equipped
	{
		NOTHING,
		C4,
		SHIELD,
		SNIPER,
		NIL,//ignore all (jihanding currently) fuck off while he finds his inner piece) 0f ass ) of 72 virgins
	}
	public Equipped equipped = Equipped.NOTHING;
	
		
	//public string Status = "walking";
	//[HideInInspector]
	public float moveSpeed = 8;		// The speed the enemy moves at.
	private float defaultSpeed = 8;
	
	
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.

	
	private int layerMsk;

	
	private Transform groundCheck;
	private Transform frontCheck2;
	
	//public GameObject bullet;
	//private bool blownup = false;
	
	public enum eStatus
	{
	
		WALKING,
		PARACHUTING,
		FALLING,
		SHOOTING,
		IDLE
		
	}
	public eStatus status = eStatus.WALKING;

	private GameObject rdGO;   //created ragdoll gameobject ref
	private Rigidbody2D rdTorsoRB; //ragdoll torso rigidbody

	private Rigidbody2D rb2D;

	private Enemy_Ragdoll rdCS; //RAGDOLL CS
	public GameObject ragdollRef; //ref for creating it
	
	private SniperSight snipeCS; //sniper code on gun;

	public  bool snipeWalk = false;
	public GameObject eDamageGOref;//prefab ref

	public AudioClip[] growlFX;

	protected override void Start()
	{

		base.Start ();
		rb2D = GetComponent<Rigidbody2D>();
		//defaultSpeed = moveSpeed;

		rdGO = Instantiate(ragdollRef, Vector3.zero, Quaternion.identity) as GameObject;

		rdCS = rdGO.GetComponent<Enemy_Ragdoll>();
		rdTorsoRB = rdGO.transform.Find ("enemy1_deadBody_body").GetComponent<Rigidbody2D>();
		rdCS.soldier = gameObject;//eSoldier;

		//rdHeadRB = rdGO.transform.Find ("head Transform").rigidbody2D;


		enemyA = GetComponent<Animator> ();
		
		headA= transform.Find ("head Transform").Find ("head").GetComponent<Animator>();
		snipeCS = transform.Find ("enemy1_deadBody_body").Find ("enemy1_deadBody_armL").Find("enemy1_deadBody_handL").Find ("sniperHand").GetComponent<SniperSight>();//headA.transform.Find ("sniperEye").Find("eyeBeam").GetComponent<SniperSight>();
		//dickMouth = headA.transform.Find ("dickMouth").gameObject;


		GameObject eDamageGO = (GameObject)Instantiate(eDamageGOref,transform.position , Quaternion.identity);//separate object needed so enemy_damage script coroutines keep working when going back and forth between enemy and ragdoll
		eDamage = eDamageGO.GetComponent<Enemy_Damage> (); 
		eDamage.enemyCS = this;

		eDamageT = transform.Find ("enemy1_deadBody_body").Find ("damage");
		eDamage.AwakeAfter(eDamageT);

		rdCS.eDamage = eDamage;//ragdoll CS references to eDamage
		rdCS.eDamageT = eDamageT;

		SortSprites();
		rdGO.SetActive(false);


		//slightly random z depth
		Vector3 pos = transform.position;
		pos.z = Random.Range (2f, 0f);
		transform.position = pos;

		//StartCoroutine("SortSprites");

		if(equipped == Equipped.C4){
			transform.Find ("enemy1_deadBody_body").Find ("c4").gameObject.SetActive(true);
		}else if(equipped == Equipped.SHIELD){
			transform.Find ("enemy1_deadBody_body").Find ("shield").gameObject.SetActive(true);
		}else if(equipped == Equipped.SNIPER){
			//print ("YAYAYA");
			snipeCS.gameObject.SetActive(true);
			status = eStatus.SHOOTING;
			enemyA.Play("enemySnipe");
			//if(!paratrooperMode){
				snipeCS.StartSniping();
			if(snipeWalk){
				InvokeRepeating("switchSnipeWalk",1,2);
			}
			rdGO.transform.Find("enemy1_deadBody_handL").Find ("sniperHand").gameObject.SetActive(true);
		}

	

		if(!facingRight && transform.localScale.x > 0){//enemy placed in level, supposed to be facing left??
			Flip();
			facingRight = false;
		}

		
		Level.enemyCount++;//for max spawn of whole level;
		


		frontCheck = transform.Find("frontCheck");
		frontCheck2 = transform.Find ("frontCheck2");

		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");
		int layerMsk1 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk3 = 1 << LayerMask.NameToLayer("OneWayGround");
		//int layerMsk4 = 1 << LayerMask.NameToLayer("Enemies");
		//int layerMsk5 = 1 << LayerMask.NameToLayer("Pickups");
		layerMsk = layerMsk1 | layerMsk2 | layerMsk3;
		//shootLayerMsk = layerMsk1 | layerMsk2 |  layerMsk3 | layerMsk4;
		groundCheck = transform.Find ("groundCheck");
		//InvokeRepeating("TestShoot", 0f, 4f);

		//parachute shit
		if(paratrooperMode){

			rdGO.SetActive(true);
			makeChute();
			switchToRagdoll();

			//rdGO.transform.Rotate (Vector3.forward * 90);
			//rdCS.makeNormalHeavy();
			rdCS.StartCoroutine("openChute",chute.transform.Find("parachute").gameObject);
			rdCS.aliveSwitch = 5f;
			rdCS.AddTorqueAll(5,20);

		}
		if (status == eStatus.IDLE) {
			enemyA.Play("enemyIdle");
		}
	}
	
	void FixedUpdate ()
	{
		
		// Create an array of all the colliders in front of the enemy.
		//Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position,layerMsk);
		
		// Check each of the colliders.
		//foreach(Collider2D c in frontHits)

		// Set the enemy's velocity to moveSpeed in the x direction.
		if(status == eStatus.WALKING){
			testFrontCheck();
			testFalling();
			rb2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rb2D.velocity.y);

			
		}else if(status == eStatus.PARACHUTING){
			testFrontCheck();
			rb2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rb2D.velocity.y);	
			testGrounded();
			
		}else if(status == eStatus.SHOOTING){
			//rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);	
			testFalling();

		}
		
	}
	void testFrontCheck()
	{
		if(Physics2D.Linecast (frontCheck.position, frontCheck2.position, layerMsk))
		{
			//print ("hit obstacle "+c.tag);
			//if(c.tag == "Obstacle")
			//{
			Flip ();
			//break;
			//}
		}
	}
	void switchToRagdoll()
	{
		//print ("SWITCH RD");
		CancelInvoke();
		headA.SetBool("ragDollTongue",true);
		headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(1);
		rdGO.SetActive(true);
		rdCS.enabled = true;
		rdGO.GetComponent<ResetPose>().Pose(new Vector3(transform.position.x,transform.position.y + 3.5f,0));//2.5f,0));
		//ragdoll references to fuck with it after switdching

		headA.gameObject.transform.parent = rdGO.transform.Find ("head Transform");
		headA.gameObject.transform.localPosition = Vector3.zero;
		headA.gameObject.transform.localScale = Vector3.one;//new Vector3(1f,1f,1f);

		eDamageT.parent = rdTorsoRB.transform;
		eDamageT.localPosition = Vector2.zero;//enemyT.Find ("enemy1_deadBody_body").transform.position;//new Vector2 (enemyT.position.x, enemyT.position.y + 1.9f);
		eDamageT.localRotation =  Quaternion.identity;
		eDamageT.localScale = Vector3.one;

		if(chute){

			harnessJoint.connectedBody = rdTorsoRB;
			harnessJoint.connectedAnchor = new Vector2(0,1.5f);
			//rdCS.makeLessHeavy();
			chute.transform.Find ("parachute").GetComponent<Parachute>().enemyCS = rdCS;
		}
	
		if(equipped == Equipped.SNIPER){//status == eStatus.SHOOTING){
			snipeCS.StopSniping();
			enemyA.Play ("enemyIdle");
		}
		rdCS.SwitchProperties (facingRight,HP, dead);



		//if(flameDamage>0f){
			//MakeSpritesBlacker();
		//}
		

		gameObject.SetActive(false);
		this.enabled = false;
	}
	public void SwitchProperties(float hp)//,Enemy_Damage eDmg)//<<----edamage ref the same still? not neccessary????
	{
		eDamage.scoreDeath = false;
		HP = hp;
		headA.SetBool("ragDollTongue",false);
		headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(0);
		if (chute) {
			harnessJoint.connectedBody = rb2D;
			harnessJoint.connectedAnchor = new Vector2(0,3.2f);	
			chute.transform.Find ("parachute").GetComponent<Parachute>().enemyCS = this;
		}
		if(parachuting){//still with open parachute
			enemyA.Play("enemyParachute");
		}else{

			//if(status == eStatus.PARACHUTING){//chute closed but switching?
				//print (defaultSpeed);
				moveSpeed = defaultSpeed;

			//}//else{
				FlipTowardsPlayer();
			//}
			enemyA.Play("GetUpAgain");
			status = eStatus.WALKING;
		}

		if(equipped == Equipped.SNIPER){
			//enemyA.Play ("enemySnipe");
			//if(status != eStatus.PARACHUTING){
				//status = eStatus.SHOOTING;
				//moveSpeed = 0;

			//}
			InvokeRepeating("switchSnipeWalk",1,7);
			//snipeCS.StartSniping();

		}
		eDamage.enemyCS = this;

	}

	public override void Shot(bool headshot, float damage,Vector2 force,Vector2 pos)
	{
		switchToRagdoll();
		rdCS.aliveSwitch = 0.2f;
		rdCS.Shot(headshot,damage,force,pos);
	}	
	public override void BlownUp(bool headshot, float damage,Vector3 ePos)
	{
		switchToRagdoll();
		rdCS.BlownUp(headshot,damage,ePos);

	}
	public override void Kicked(bool headshot, float damage,Vector3 ePos)
	{
		switchToRagdoll();
		rdCS.Kicked(headshot,damage,ePos);

	}
	public override void Damaged(bool headshot, float damage)
	{
		switchToRagdoll();
		rdCS.Damaged(headshot,damage);

	}
	
	public override void Flamed(){
		base.Flamed ();
			enemyA.SetBool ("onFire",true);
			//MakeSpritesBlacker();

	}
	public override void FlameDamage(float dmg)
	{
		base.FlameDamage(dmg);
		if (died) {
			switchToRagdoll ();//only death that hasnt already switched to ragdoll mode;
			rdCS.Death();
			//Death();
		}
	}
	public override void StopFlame()
	{
		enemyA.SetBool ("onFire", false);
	}

	public override Rigidbody2D Grabbed()
	{
		switchToRagdoll ();
		rdCS.aliveSwitch = float.MaxValue;
		rdCS.grabbed = true;
		if(!parachuting){
			rdCS.makeLessHeavy ();
		}
		return rdCS.body.GetComponent<Rigidbody2D> ();
	}
	public override void Tripped(bool right,Vector2 vel)
	{
		//print ("tripped "+onFire);
		// Reduce the number of hit points by one.
		
		Instantiate(hitEffect, transform.position, Quaternion.identity);
		float power = 1000f;
		if(right){
			power = -1000f;
		}
		switchToRagdoll ();
		rdCS.aliveSwitch = float.MaxValue;
		//Rigidbody2D bdy = ragdollBody.transform.Find("enemy1_deadBody_body").rigidbody2D;
		rdGO.transform.Find ("enemy1_deadBody_foot").GetComponent<AudioSource> ().Play ();
		//set damage
		rdTorsoRB.AddForce(new Vector2(power,Mathf.Abs(vel.x) * 800f));
		rdTorsoRB.AddTorque(vel.y * power);

	}

	public override void HurtPlayer(Vector3 pos)
	{
		FlipTowardsPlayer();
		pos.y += 2f;
		Vector3 charPos = headA.transform.position;
		
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
		headA.transform.rotation = angle3;
		if(equipped == Equipped.C4 && !jihadding){
			
			StartCoroutine("Jihad");
			
		}else if(equipped != Equipped.NIL){//jihadding, ignore
			//Destroy(enemyA);
			headA.SetTrigger("DickMouth");
			headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(3);
			StartCoroutine("StopHurtPlayer");

			AudioSource.PlayClipAtPoint(growlFX[UnityEngine.Random.Range (0, growlFX.Length)], transform.position);
		}
	}
	IEnumerator StopHurtPlayer()
	{
		yield return new WaitForSeconds(.5f);
		headA.transform.rotation = Quaternion.identity;
		headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(0);
	}
	IEnumerator Jihad()
	{
		
		jihadding = true;//ignore shit next time he hurts player
		//enemyCS.moveSpeed = 0f;
		enemyA.Play("enemySuicideBomb");
		headA.SetBool("ragDollTongue",true);
		headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(1);
		yield return new WaitForSeconds(1.2f);
		
		


		dead = true;
		switchToRagdoll();

		eDamage.scoreDeath = false;

		rdCS.Death();
		//if he still has his C4 and whatever mode hes in mofo
		//print (rdTorsoRB.transform.Find("c4").Find ("explosive"));
		if(rdTorsoRB.transform.Find("c4")){
			rdTorsoRB.transform.Find("c4").GetComponent<Explosive>().enemyExplosion = true;
			rdTorsoRB.transform.Find("c4").GetComponent<Explosive>().Explode();
			rdTorsoRB.transform.Find("c4").GetComponent<SpriteRenderer>().enabled = false;
			eDamage.damageHead(4);
			rdTorsoRB.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getBody(2);
		}
		rdTorsoRB.AddTorque (Random.Range(1000f,2000f));
		rdCS.DestroyLimbs(new Vector2 (Random.Range(-200,200),Random.Range(-200,200)));

		//Death ();
	}
	public override void Bounce(Vector2 vel)
	{
		//if(facingRight){
			//Flip();
		//}
		switchToRagdoll ();
		rdCS.Bounce (vel);


	}
	/*void FixedUpdate()

	{
		if(!dead){
			print (parachuting);
		}
	}*/
	
	



	
	
	
	void testGrounded()
	{
		//print ("TEST GROUND "+Time.deltaTime);
		if(Physics2D.Linecast (transform.position, groundCheck.position, layerMsk)){
			//print ("HIT GROUND "+defaultSpeed);
			moveSpeed = defaultSpeed;
			if(equipped == Equipped.SNIPER){
				status = eStatus.SHOOTING;
			}else{
				status = eStatus.WALKING;
				enemyA.Play ("enemyWalk");
			}
			
			closeChute();
			//StartCoroutine("delayAnim");
		}

	}
	//IEnumerator delayAnim()
	//{
		//yield return new WaitForSeconds(0.01f);
		//enemyA.Play ("enemyWalk");
	//}
	void testFalling()
	{
		if(rb2D.velocity.y < -5f){
			bool grounded =Physics2D.Linecast (transform.position, groundCheck.position, layerMsk);  
			//print ("paracuting "+grounded);
			if(!grounded){
				var vel = rb2D.velocity;
				status = eStatus.FALLING;
				//startFalling();
				switchToRagdoll();
				rdCS.aliveSwitch = float.MaxValue;
			
				//print (vel);
				rdTorsoRB.velocity = vel;//,ForceMode2D.Impulse);
				rdCS.AddTorqueAll (-80,80);
			}
		}
	}
	/*public void startFalling()
	{
		switchToRagdoll();
		//rdCS.AddTorqueAll (-50,50);
	}*/
	void makeChute()
	{
		chute = Instantiate(chuteRef, new Vector3(transform.position.x,transform.position.y,0.1f), Quaternion.identity) as GameObject;
		parachuting = true;
		chute.transform.Find ("parachute").GetComponent<Parachute>().enemyCS = this;
		//chute.transform.parent = eDamage.transform;//so it gets destroyed

		//attach ragdoll
		harnessJoint = chute.transform.Find("harness").gameObject.AddComponent<HingeJoint2D>();
		//harnessJoint.connectedBody = rdTorsoRB;	
		harnessJoint.useLimits = true;
		JointAngleLimits2D limits = harnessJoint.limits;
		limits.min = -10f;
		limits.max = 10f;
		harnessJoint.limits = limits; 

		status = eStatus.PARACHUTING;
		//enemyA.Play("enemyParachute");
	
		chute.transform.Find ("parachute").gameObject.SetActive(false);


	}


	public override void chuteHurt(bool fire)
	{
		switchToRagdoll();
		parachuting = false;
		rdCS.AddTorqueAll(-30,30);
		rdCS.AddXForceAll(rb2D.velocity*1.5f);
		//rdCS.makeNormalHeavy();
		rdCS.aliveSwitch = float.MaxValue;
		//status = eStatus.FALLING;
	}
	private void closeChute()
	{
		//print ("close Chute");
		//base.closeChute ();
		if(chute){
			parachuting = false;
			chute.transform.Find ("parachute").GetComponent<Parachute>().closeChute();
		}
	}
	
	void switchSnipeWalk()
	{
		if(status == eStatus.PARACHUTING){return;}
		if(status == eStatus.WALKING){
			FlipTowardsPlayer();
			status = eStatus.SHOOTING;
			snipeCS.StartSniping();
			enemyA.Play ("enemySnipe");
		}else{

			snipeCS.StopSniping();
			enemyA.Play ("enemyWalk");
			status = eStatus.WALKING;
		}
		//print ("SWIIIIIIIIIIIITCH  "+status);
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
	void SortSprites()
	{
		//yield return new WaitForSeconds(0.01f);
		sortingNum = Level.instance.sortingNumber+=18;//UpdateSorting();
		//print (sortingNum+"  "+Level.instance.sortingNumber);
		if(parachuting){
			
			chute.transform.Find("parachute").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
			chute.transform.Find("harness").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
			//Level.instance.sortingNumber++;
			//sortingNum++;
		}
	
		Transform t = headA.transform.Find ("dickMouth");
		t.Find("mouthTOP").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("mouthBOT").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("dick").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("tongue").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;



		t= transform.Find("enemy1_deadBody_body");
		t.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_armL").Find("enemy1_deadBody_handL").Find("sniperHand").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;

		t.Find("ass").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("pants").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_arm").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_arm").Find("enemy1_deadBody_hand").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_armL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_armL").Find("enemy1_deadBody_handL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_leg").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_leg").Find("enemy1_deadBody_foot").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_legL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find("enemy1_deadBody_legL").Find("enemy1_deadBody_footL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		headA.transform.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		foreach (Transform child in eDamageT.Find("bulletHoles")) {
			child.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
			//child.SetActive(false);
		}
		foreach (Transform child in headA.transform.Find ("headDamage")){
			child.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
			//child.SetActive(false);
		}
		
		t.Find ("c4").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		t.Find ("shield").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		//print(sortingNum);
		eDamageT.Find ("flame").GetComponent<SetParticleSortingLayer>().sortingOrder += sortingNum;
		eDamageT.Find ("smoke").GetComponent<SetParticleSortingLayer>().sortingOrder += sortingNum;
		rdCS.SortSprites(sortingNum);//////////ragdoll sorting
	}
	
	
}

