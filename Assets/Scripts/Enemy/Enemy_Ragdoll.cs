 using UnityEngine;
using System.Collections;


public class Enemy_Ragdoll : Enemy 
{

	//private bool facingRight = true;

	//public GameObject soldierRef;//enemy ref for creating enemy soldier
	[HideInInspector]
	public GameObject soldier; //soldier that created the ragdoll, if ragdoll is alive



	public GameObject deadRob;
	//private ParticleSystem neckBlood;

	public Transform body;

	public GameObject chute = null;

	private float aliveCount = 0f;  ///how long has he been alive
	public float aliveSwitch = 5f;//how long before he switches to upright soldier


	private bool hittable = true; //is he hittable still?
	private bool headLess = false; //headless ? :)


	public bool grabbed = false;
	public AudioClip[] deathFX;

	private Enemy_Ragdoll_DamageCollider dColliderH;
	private Enemy_Ragdoll_DamageCollider dColliderB;

	void Awake() 
	{
	
		body = transform.Find("enemy1_deadBody_body");
		dColliderH = transform.Find("head Transform").GetComponent<Enemy_Ragdoll_DamageCollider>();
		dColliderB = body.GetComponent<Enemy_Ragdoll_DamageCollider>();
		//if (dead) {
			//StartCoroutine(makeUnHittable());
		//}

	}
	public void LateAwake()
	{
		Enemy_Soldier enemycs = soldier.GetComponent<Enemy_Soldier>();
		equipped = enemycs.equipped;
		if(equipped == Enemy_Soldier.Equipped.C4){
			body.Find ("c4").gameObject.SetActive(true);
		}else if (equipped == Enemy_Soldier.Equipped.SHIELD){
			body.Find("shield").gameObject.SetActive(true);
		}

	}
	void FixedUpdate ()
	{
		//float speed2 = Mathf.Abs (body.rigidbody2D.velocity.x) + Mathf.Abs (body.rigidbody2D.velocity.y);
		//print (Time.time);
		//return;
		if (hittable) {
			aliveCount += Time.deltaTime;
			if(aliveCount > 0.1f){//keep alive for a sec so speed isnt zero
				if (aliveCount > aliveSwitch) {
					//print ("enemy RD SWITCHD ");
					switchToEnemy();
					return;
				}
				float speed = Mathf.Abs (body.GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (body.GetComponent<Rigidbody2D>().velocity.y);
				///print (speed);
				if (speed < 0.05) {
					//print ("enemy RD STOPPED");
					if(dead){
						StartCoroutine("makeUnHittable");
					}else if(!grabbed){
						//soldier.SetActive(true);
						//soldier.GetComponent<Animator>().Play("GetUpAgain");
						switchToEnemy();

					}
				}
			}

		}//else if(hittable){ //dead but still hittable?
			//float speed = Mathf.Abs (body.rigidbody2D.velocity.x) + Mathf.Abs (body.rigidbody2D.velocity.y);
			//print (speed);
			//if (speed < 0.05) { //stationary?
				
			//}
		//}

	}
	void switchToEnemy()
	{
		
		//print ("switch enemy");

		//return;
		//GameObject soldier = Instantiate(soldierRef, new Vector3(transform.position.x,transform.position.y + 2.5f,transform.position.z), Quaternion.identity) as GameObject;
		//print (dead);
		//if(!soldier){
			//Level.instance.pauser.Pause();
		//}
		soldier.transform.position = new Vector2(body.position.x - .5f ,body.position.y - 2);
		Transform hed = transform.Find ("head Transform").Find ("head");
		//print (hed);
		hed.parent = soldier.transform.Find("head Transform");
		hed.localPosition = new Vector3(0f,0f,0f);
		hed.localRotation =  Quaternion.identity;
		hed.localScale = transform.localScale;

		soldier.SetActive(true);

		eDamageT.parent = soldier.transform.Find ("enemy1_deadBody_body");
		eDamageT.localPosition = Vector2.zero;//enemyT.Find ("enemy1_deadBody_body").transform.position;//new Vector2 (enemyT.position.x, enemyT.position.y + 1.9f);
		eDamageT.localRotation =  Quaternion.identity;
		eDamageT.localScale = Vector3.one;


		soldier.GetComponent<Enemy_Soldier> ().enabled = true;
		soldier.GetComponent<Enemy_Soldier> ().SwitchProperties (HP);

		//makeUnHittable();
		//Destroy(this);
		//Destroy (gameObject);
		gameObject.SetActive(false);
		this.enabled = false;
	}
	public void SwitchProperties(bool facingR,float hp, bool ded)
	{

		HP = hp;
		dead = ded;
		//print ("sw "+dead);
		aliveCount = 0;
		//eDamage.bodyBlood = body.transform.Find("bodyBlood").GetComponent<ParticleSystem>();
		if (facingR) {
			Flip ();
		}
		eDamage.enemyCS = this;

		//check if C4 is gone
		Enemy_Soldier enemycs = soldier.GetComponent<Enemy_Soldier>();
		//print (equipped+"   "+enemycs.equipped);
		if(equipped != enemycs.equipped){
			body.Find ("c4").gameObject.SetActive(false);
			equipped = Equipped.NOTHING;
		}
		dColliderB.fallActivated = true;
		dColliderH.fallActivated = true;
		dColliderB.fallTime = dColliderH.fallTime = Time.time;
		//if(!enemycs.facingRight){
			//Flip ();
		//}
	}
	public override void Shot(bool headshot, float damage,Vector2 force,Vector2 pos)
	{

		base.Shot (headshot, damage,force,pos);
		
		//if(parachuting){
		//extraDmg = 3f;
		//}
		if(headshot){
			eDamage.damageHead(damage);
		}else{
			eDamage.makeBodyBulletHole();
		}
		body.GetComponent<Rigidbody2D>().AddForceAtPosition(force,pos,ForceMode2D.Impulse);
		aliveCount = 0;

		if (died) {
			Death();
			//makeNormalHeavy();
			if(headshot){
				eDamage.destroyHead();
				destroyHead();
			}
		}

	}
	public override void BlownUp(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){

			base.BlownUp(headshot,damage,ePos);
			if(headshot){
				Level.instance.makeHitFX(transform.Find ("head Transform").position);
			}else{
				Level.instance.makeHitFX(body.position);
			}
			aliveSwitch = float.MaxValue;
			//var bForce = damage * 100/12;
			Vector3 force = GetBlastDirection(body.position,ePos);
			//print ("rd blownup damage "+force);
			var d = GetBlastTorque(force);
			
			//rdTorsoRB.AddForceAtPosition (force * bForce * 120,force,ForceMode2D.Impulse);
			//damage *= 40;
			//print ("blown up RD "+died);
			body.GetComponent<Rigidbody2D>().AddForce (force * damage * 10 ,ForceMode2D.Impulse);
			body.GetComponent<Rigidbody2D>().AddTorque(damage * d);
			//body.GetComponent<Rigidbody2D>().AddForceAtPosition (force *damage ,force,ForceMode2D.Impulse);//force);
			if (died) {
				Death();
				DestroyLimbs(force);
				if(damage>8){
					body.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getBody(2);
					eDamage.damageHead(3);
				}else if(damage>5){
					body.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getBody(1);
					eDamage.damageHead(2);
				}else{
					eDamage.damageHead(1);
				}
			}
		}
		
	}
	public override void Kicked(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){
			
			base.BlownUp(headshot,damage,ePos);
			if(headshot){
				Level.instance.makeHitFX(transform.Find ("head Transform").position);
			}else{
				Level.instance.makeHitFX(body.position);
			}
			aliveSwitch = float.MaxValue;
			//var bForce = damage * 100/12;
			Vector3 force = GetBlastDirection(body.position,ePos);
			//print ("rd blownup damage "+force);
			var d = GetBlastTorque(force);
			force.y = damage/2;
			//rdTorsoRB.AddForceAtPosition (force * bForce * 120,force,ForceMode2D.Impulse);
			//damage *= 40;
			//print ("rd torque "+damage * d);
			body.GetComponent<Rigidbody2D>().AddForce (force * damage * 40 ,ForceMode2D.Impulse);
			body.GetComponent<Rigidbody2D>().AddTorque(damage * d);
			//body.GetComponent<Rigidbody2D>().AddForceAtPosition (force *damage ,force,ForceMode2D.Impulse);//force);
			if (died) {
				Death();
				
				//DestroyLimbs(force);
			}
		}
		
	}
	public override void Damaged(bool headshot, float damage, Vector2 vel)
	{
		
		if(!eDamage.blownUp){
			//print ("rd blownup damage "+damage+"  "+force);
			base.Damaged(headshot,damage,vel);
			//makeNormalHeavy();
			aliveSwitch = float.MaxValue;

			if (died) {
				Death();
				
				DestroyLimbs(Vector2.up);
				if(damage>8){
					//rdTorsoRB.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getBody(2);
					eDamage.damageHead(3);
				}else if(damage>5){
					//rdTorsoRB.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getBody(1);
					eDamage.damageHead(2);
				}else{
					eDamage.damageHead(1);
				}
			}
		}
		
	}

	public override void FlameDamage(float dmg)
	{
		base.FlameDamage(dmg);
		if (died) {
			Death();
		}
	}
	public void DestroyLimbs(Vector2 vel)
	{
		//print ("limbsssssssssss");
		//return;
		HingeJoint2D[] joints = body.GetComponents<HingeJoint2D>();
		foreach (HingeJoint2D jnt in joints) {
			Rigidbody2D limb = jnt.connectedBody; 
			if(limb){
				//limb.AddForce(vel/2f);           ////jostle the limbs a bit anyway

				if (Random.value > 0.7)
				{

					bool destroyed = false;
					if(jnt.connectedBody.name == "enemy1_deadBody_armL"){
						limb.transform.Find("stump").gameObject.SetActive(true);
						Destroy (jnt);
						body.Find("stump_armL").gameObject.SetActive(true);
						limb.tag = "EnemyRDLimb";
						transform.Find ("enemy1_deadBody_handL").tag = "EnemyRDLimb";
						destroyed = true;
					}else if(jnt.connectedBody.name == "enemy1_deadBody_arm"){
						limb.transform.Find("stump").gameObject.SetActive(true);
						Destroy (jnt);
						body.Find("stump_armR").gameObject.SetActive(true);
						limb.tag = "EnemyRDLimb";
						transform.Find ("enemy1_deadBody_hand").tag = "EnemyRDLimb";
						destroyed = true;
					}else if(jnt.connectedBody.name == "enemy1_deadBody_legL"){
						limb.transform.Find("stump").gameObject.SetActive(true);
						Destroy (jnt);
						body.Find("stump_legL").gameObject.SetActive(true);
						limb.tag = "EnemyRDLimb";
						transform.Find ("enemy1_deadBody_footL").tag = "EnemyRDLimb";
						destroyed = true;
					}else if(jnt.connectedBody.name == "enemy1_deadBody_leg"){
						limb.transform.Find("stump").gameObject.SetActive(true);
						Destroy (jnt);
						body.Find("stump_legR").gameObject.SetActive(true);
						limb.tag = "EnemyRDLimb";
						transform.Find ("enemy1_deadBody_foot").tag = "EnemyRDLimb";
						destroyed = true;
					}
					if(destroyed){
						//print ("destoyee");
						var vell = Vector2.up * 3000;
						vell.x += Random.Range(-500,500);
						limb.AddForce(vell);//+(vel/2f)); 
					}
				}
			}
		}
	}
	public override void RemoveC4()
	{
		equipped = Equipped.NOTHING;
		if (!dead) {
			soldier.GetComponent<Enemy_Soldier> ().RemoveC4 ();
		}

	}
	public override void Death()
		{
		base.Death ();
		
		// Increase the bodycount
		//Level.instance.addBodyCount(gameObject);
	
		// Play a random audioclip from the deathClips array.
		int i = Random.Range(0, deathFX.Length);
		AudioSource.PlayClipAtPoint(deathFX[i], transform.position);
		

		if(!headLess){
			var h = transform.Find ("head Transform").Find("head");
			Destroy(h.GetComponent<Animator>());
			h.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(2);
			Destroy (dColliderH);
		}

			
		Destroy (dColliderB);
		if(equipped == Equipped.C4){
			body.Find ("c4").GetComponent<Explosive>().xMode = Explosive.XplodeMode.DEADENEMY;//gameObject.layer = LayerMask.NameToLayer("Bodies");

		}
//		makeLessHeavy();
		//body.rigidbody2D.AddForce(vel);
		aliveSwitch = float.MaxValue;
		if(eDamage.scoreDeath){
			Level.instance.score.UpdateScore(100,new Vector3(body.position.x,body.position.y+8f));
		}
		Enemy_Soldier es = soldier.GetComponent<Enemy_Soldier>();

		if(es.replenishSpawner && es.spawner){// != null){
			es.spawner.transform.GetComponent<Spawner>().subtractSpawnCount(1);
		}
		Level.instance.enemies.Remove(es); // ragdoll CS (this) is removed from enemies List in unHittable
		//StartCoroutine(makeUnHittable());
		if(grabbed){
			Level.instance.StopPlayerGrab();
		}
		Destroy (soldier);

	}

	public void destroyHead()
	{

		Transform headTT = transform.Find ("head Transform");
		headTT.transform.Find ("head").gameObject.SetActive(false);
		Destroy(headTT.GetComponent<HingeJoint2D>());
		Destroy(headTT.GetComponent<BoxCollider2D>());
		Destroy(headTT.GetComponent<Rigidbody2D>());
		//headT.GetComponent<SpriteRenderer>().enabled = false;

		GameObject deadHead = (GameObject)Instantiate(deadRob,transform.Find ("head Transform").transform.position , Quaternion.identity);
		deadHead.GetComponent<DeadRob>().sortingNum = sortingNum+3;
		headLess = true;
	}

	public override void Bounce(Vector2 vel)
	{
		//if(facingRight){
			//Flip();
		//}
		aliveSwitch = float.MaxValue;
		//body.rigidbody2D.velocity = Vector2.zero;
		body.GetComponent<Rigidbody2D>().velocity = vel;//(vel);
		if(!headLess){
			transform.Find ("head Transform").GetComponent<Rigidbody2D>().velocity = vel;
		}
	}
	/*
	public void AddXForceAll(Vector2 vel){
		foreach (Transform child in transform) {
			//child.parent = null;
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			//child.transform.rotation = randomRotation;
			if(child.GetComponent<Rigidbody2D>()){
				Vector2 vell = child.GetComponent<Rigidbody2D>().velocity;
				vell.x = vel.x;
				child.GetComponent<Rigidbody2D>().velocity = vell;
			}
		}
		
		
		
	}*/
	public void AddTorqueAll(int vel,int vel2){
		foreach (Transform child in transform) {
			//child.parent = null;
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			//child.transform.rotation = randomRotation;
			if(child.GetComponent<Rigidbody2D>()){
				child.GetComponent<Rigidbody2D>().AddTorque(Random.Range(vel,vel2),ForceMode2D.Impulse);
			}

		}
		body.GetComponent<Rigidbody2D>().AddTorque(100f * Random.Range(vel,vel2));
		
		
	}
	public void makeNormalHeavy(){
		foreach (Transform child in transform) {
			//child.parent = null;
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			//child.transform.rotation = randomRotation;
			if(child.GetComponent<Rigidbody2D>()){
				child.GetComponent<Rigidbody2D>().mass =1f;
				child.GetComponent<Rigidbody2D>().gravityScale = 1f;
			}
		}



	}
	public void makeLessHeavy(){
		foreach (Transform child in transform) {
			//child.parent = null;
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			//child.transform.rotation = randomRotation;
			if(child.GetComponent<Rigidbody2D>()){
				child.GetComponent<Rigidbody2D>().mass = 0.001f;
				child.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
			}
		}
		
		
		
	}
	/*
	public void makeHeadHeavy(){
		if(headLess){return;}
		transform.Find ("head Transform").GetComponent<Rigidbody2D>().mass = 4f;
		transform.Find ("head Transform").GetComponent<Rigidbody2D>().gravityScale = 1.2f;
	}*/
	private IEnumerator makeUnHittable(){

		Level.instance.addBodyCount(gameObject);//add to body management system
		Level.instance.enemies.Remove(this);
		transform.position = new Vector3(transform.position.x,transform.position.y,1);
		hittable = false;
		/*if(!headLess){

			Destroy (transform.Find("head Transform").Find("head").GetComponent<Animator>());


		}*/


		yield return new WaitForSeconds(.3f);

		//push bodies back a  bit
		//Vector3 v = transform.position;
		//v.z +=2f;
		//transform.position = v;
		if(chute){
			chute.transform.Find("parachute").GetComponent<Parachute>().Hurt(false);
			chute.transform.Find ("harness").GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		}
		//put them on bodies layer
		foreach (Transform child in transform) {
			child.gameObject.layer = 21;
			child.tag = "Untagged";
			//if(child.rigidbody2D){
				//child.rigidbody2D.isKinematic = true;
			//}
		}
		transform.Find("head Transform").gameObject.layer = 21;
		transform.Find("head Transform").gameObject.tag = "Untagged";
		body.Find("shield").gameObject.layer = 21;

		foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {//foreach (Transform child in transform) {
			sr.sortingLayerName = "Background";

		}

		GetComponent<FadeColour>().enabled = true;

		eDamage.unHittable();//destroys eDamage gameobject, waits for fire if need be;
		Destroy(this);//destroy this script
	}
	public IEnumerator openChute()
	{
		var go = chute.transform.Find("parachute").gameObject;
		Animator anim = go.GetComponent<Animator>();
		yield return new WaitForSeconds(soldier.GetComponent<Enemy_Soldier>().openChuteDelay);
		Transform harnessT = go.GetComponent<DistanceJoint2D> ().connectedBody.transform;
		Vector3 harnessPos = harnessT.position;
		harnessPos.y += 4f;
		harnessT.rotation = Quaternion.identity;
		go.transform.position = harnessPos;

		anim.Play ("Parachute_shot");
		//makeLessHeavy();
		body.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		//body.rigidbody2D.AddForce(new Vector2(0,0f));//.up * 10f);
		go.SetActive(true);
		yield return new WaitForSeconds(0.7f);
		anim.Play ("Parachute Bounce");
		//soldier.SetActive(true);
		//soldier.rigidbody2D.AddForce(Vector2.up * 3f);
		if(!dead){
			//makeNormalHeavy();
		//}else{
			switchToEnemy();
		}//else{
			//soldier.GetComponent<Enemy_Soldier>().closeChute();
		//}

	}
	public override void chuteHurt()
	{
		//makeNormalHeavy();
		print ("CHUTE DONE");
		aliveSwitch = float.MaxValue;
		body.GetComponent<Rigidbody2D>().AddTorque(50f,ForceMode2D.Impulse);
		//if(soldier){
			//soldier.GetComponent<Enemy_Soldier>().status = Enemy_Soldier.eStatus.FALLING;
		//}
	}
	/*public IEnumerator delaySwitchToEnemy(float delay)
	{
		
		// Wait for 2 seconds.
		yield return new WaitForSeconds(delay);
		if(!dead){// && parachuting && ragdollMode){
		print ("delayed switch enemy");
		switchToEnemy();
		}
	}*/
	public void Flip()
	{
		//print ("flip");
		facingRight = !facingRight;
		Transform hd = transform.Find ("head Transform");
		Vector3 enemyScale = hd.localScale;
		enemyScale.x *= -1;
		hd.localScale = enemyScale;
	}

	public void SortSprites(int sNum)
	{
		sortingNum = sNum;
		body.Find("stump_armR").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		body.Find("stump_armL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		body.Find("stump_legL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		body.Find("stump_legR").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_leg").Find("stump").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_legL").Find("stump").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_arm").Find("stump").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_armL").Find("stump").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;

		body.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_arm").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_hand").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_armL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_handL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_leg").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_foot").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_legL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		transform.Find("enemy1_deadBody_footL").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		body.Find("c4").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		body.Find("shield").GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
		//transform.Find("head Transform").Find("head").GetComponent<SpriteRenderer>().sortingOrder = sortingNum+2;
		//print(sortingNum+  "  RD");
	}
}
