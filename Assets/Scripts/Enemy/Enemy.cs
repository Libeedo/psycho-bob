using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy:MonoBehaviour {
	//private GameObject enemyGO; //upright enemy game object stuff ref
	//private Transform enemyT;
	//private Enemy enemyCS;
	

	
	
	//[HideInInspector]
	public float HP = 10f;
	//private float maxHP;// How many times the enemy can be hit before it dies.


	
	protected bool dead = false;
	protected bool died = false; //used to check if just died.


	[HideInInspector]
	public Enemy_Damage eDamage;//the damage script on a separtate gameobject
	[HideInInspector]
	public Transform eDamageT; //the damage fx on his chest transform


	//protected Score score;
	[HideInInspector]
	public GameObject spawner;
	[HideInInspector]
	public bool replenishSpawner = false;
	
	public int sortingNum;
	

	public bool facingRight = true;

	public GameObject hitEffect;

	protected virtual void Start(){
		//print ("START");
		Level.instance.enemies.Add(this); //base class adds soldier or ragdoll child scripts
		//maxHP =   HP;
		//score = ;//GameObject.Find("Score").GetComponent<Score>();
		//enemyGO = transform.Find("enemy").gameObject;
		//enemyT = transform.Find ("enemy").transform;
		//enemyCS = transform.Find ("enemy").GetComponent<Enemy>();

	}
	public enum Equipped
	{
		NOTHING,
		C4,
		SHIELD,
		SNIPER,
		NIL,//ignore all (jihanding currently) fuck off while he finds his inner piece) 0f ass ) of 72 virgins
	}
	public Equipped equipped = Equipped.NOTHING;
	
	void takeDamage (float dmg) //returns ture if dead;
	{
		//dmg*=10f;
		HP -= dmg;
		//print ("hurt   "+HP);
		if(!dead){
			Level.instance.makeHitNum((Vector2)eDamageT.position+Vector2.up*3,dmg);
			if(HP < 0f){
				dead = true;
				died = true;
			}
		
		}
	
	}
	

	public virtual void Shot(bool headshot, float damage,Vector2 force, Vector2 pos)
	{
		eDamage.scoreDeath = true;
		takeDamage (damage);

	}	
	public virtual void BlownUp(bool headShot,float damage,Vector3 ePos)
	{

		eDamage.scoreDeath = true;
			takeDamage (damage);
			eDamage.BlownUp ();
			
	}
	public virtual void Kicked(bool headShot,float damage,Vector3 ePos)
	{
		
		eDamage.scoreDeath = true;
		takeDamage (damage);
		eDamage.BlownUp ();
		
	}
	public virtual void Damaged(bool headShot,float damage)
	{
		takeDamage (damage);
		//eDamage.BlownUp ();
	}
	
	public virtual void Flamed(){
		eDamage.scoreDeath = true;
		eDamage.Flamed ();
	
	}
	public virtual void FlameDamage(float dmg)
	{
		takeDamage (dmg);

	}
	public virtual void StopFlame()
	{
		
	}
	public virtual void chuteHurt(bool fire)
	{

	}
	public virtual void Bounce(Vector2 vel)
	{

	}

	public virtual Rigidbody2D Grabbed()
	{
		return null;
	}
	/*private void MakeSpritesBlacker()
	{
		Color clr = new Color(flameDamage,flameDamage,flameDamage,1f);
		//print (flameDamage);
		if (ragdollMode) {
			foreach (SpriteRenderer sr in rdGO.GetComponentsInChildren<SpriteRenderer>()) {
				sr.material.color = clr;
			}
		} else {
			headA.transform.Find ("dickMouth").gameObject.SetActive (true);
			foreach (SpriteRenderer sr in enemyT.GetComponentsInChildren<SpriteRenderer>()) {
				sr.material.color = clr;
			}
			
			enemyT.transform.Find ("enemy1_deadBody_body").transform.Find ("ass").GetComponent<SpriteRenderer>().material.color = clr;
			enemyT.transform.Find ("enemy1_deadBody_body").transform.Find ("pants").GetComponent<SpriteRenderer>().material.color = clr;
			headA.transform.Find ("dickMouth").gameObject.SetActive (false);
		}
	}*/
	public virtual void Tripped(bool right,Vector2 vel)
	{
		//print ("tripped "+onFire);
		// Reduce the number of hit points by one.
		
		
		/*float power = 1000f;
		if(right){
			power = -1000f;
		}
		switchToRagdoll ();
		//Rigidbody2D bdy = ragdollBody.transform.Find("enemy1_deadBody_body").rigidbody2D;
		rdGO.transform.Find ("enemy1_deadBody_foot").GetComponent<AudioSource> ().Play ();
		//set damage
		rdTorsoRB.AddForce(new Vector2(power,Mathf.Abs(vel.x) * 800f));
		rdTorsoRB.AddTorque(vel.y * power);*/
		
	}
	public virtual void Death()
	{
		//print ("enemy dead");
		//dead = true;
		died = false;
		/*if (!ragdollMode){
			

			switchToRagdoll ();
			
		}
		if(onFire){
			rdGO.transform.Find ("enemy1_deadBody_body").transform.Find("flame").GetComponent<ParticleSystem>().Play ();
		}
		if(blownUp){
			rdCS.DestroyLimbs(vel);
			if (Mathf.Abs(vel.x)+Mathf.Abs(vel.y) > 4500f){
				rdTorsoRB.GetComponent<SpriteRenderer>().sprite = jihadBody;
			}
		}
		rdCS.Death(vel);*/
		if(replenishSpawner && spawner){// != null){
			spawner.transform.GetComponent<Spawner>().subtractSpawnCount(1);
		}
		//if(scoreDeath){
			//score.UpdateScore(100,new Vector3(rdTorsoRB.transform.position.x,rdTorsoRB.transform.position.y+8f));
		//}
		Level.enemyCount--;
		//print ("remove");

	}

	public virtual bool HurtPlayer(Vector3 pos)
	{
		return true;
	}
	protected Vector3 GetBlastDirection(Vector3 pPos,Vector3 ePos)
	{
		// Find a vector from the bomb to the enemy.
		
		Vector3 deltaPos = pPos - ePos;
		
		// Apply a force in this direction with a magnitude of bombForce.
		Vector3 force = deltaPos.normalized;
		//Debug.DrawLine(deltaPos.normalized,transform.position, Color.red,10f);
		return force;
		//col.AddForce(force);
		//print ("hit enemy");
		
	}
	protected int GetBlastTorque(Vector3 force)
	{
		var d = 8000;
		if(force.y > 0){ 
			//force.y *= damage/2; //push higher up if direction is up at all
			if(force.x < 0){//get  correct torque, based on if its above/below,left/right
				d = -8000;
			}
		}else{
			if(force.x >0){
				d = -8000;
			}
		}
		return d;
	}
	
	

}
