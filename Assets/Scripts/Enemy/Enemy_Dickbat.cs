 using UnityEngine;
using System.Collections;

public class Enemy_Dickbat : Enemy {
	//private Enemy_Damage eDamage;
	//private Transform db;
	protected override void Start()
	{
		base.Start ();
		//db = transform.Find("dickBat");
		eDamageT = transform.Find("eDamageGO");
		eDamage = transform.Find("eDamageGO").GetComponent<Enemy_Damage>();
		eDamage.enemyCS = this;

		eDamage.AwakeAfter(transform.Find ("damage"));
		if (transform.Find ("sniperHand")) {
			transform.Find ("sniperHand").GetComponent<SniperSight>().StartSniping();
		}
	}

	public override void Shot(bool headshot, float damage,Vector2 force, Vector2 pos)
	{

		eDamage.makeBodyBulletHole();
		base.Shot (headshot, damage,force,pos);
		if (died) {
			Death();
		}
	}
	public override void BlownUp(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){//if not just blownup
			Instantiate(hitEffect, transform.position, Quaternion.identity);
			base.BlownUp(headshot,damage,ePos);
			if (died) {
				Death();
			}
		}

	}
	public override void Kicked(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){//if not just blownup
			Instantiate(hitEffect, transform.position, Quaternion.identity);
			base.BlownUp(headshot,damage,ePos);
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
		GetComponent<Rigidbody2D>().isKinematic = false;
		GetComponent<Animator>().Play ("DickBat_Death");
		Destroy (GetComponent<Drone>());
		Destroy (GetComponent<BoxCollider2D>());
		if(transform.Find ("dWeapon")){
			transform.Find ("dWeapon").GetComponent<SniperSight>().enabled = false;
		}
		Destroy (gameObject,1.5f);
	}

}
