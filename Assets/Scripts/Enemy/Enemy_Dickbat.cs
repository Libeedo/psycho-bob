 using UnityEngine;
using System.Collections;

public class Enemy_Dickbat : Enemy {
	private Transform dickT;
	private Animator dickA;

	private bool canAttack = true;

	public AudioClip[] growlFX;
	private Drone droneCS;
	protected override void Start()
	{
		base.Start ();
		//db = transform.Find("dickBat");
		eDamageT = transform.Find("eDamageGO");
		eDamage = transform.Find("eDamageGO").GetComponent<Enemy_Damage>();
		eDamage.enemyCS = this;

		eDamage.AwakeAfter(transform.Find ("damage"));
		dickT = transform.Find("dWeapon");
		dickA = dickT.GetComponent<Animator>();

		droneCS = GetComponent<Drone> ();

		//if (transform.Find ("sniperHand")) {
			//transform.Find ("sniperHand").GetComponent<SniperSight>().StartSniping();
		//}
	}

	public override void Shot(bool headshot, float damage,Vector2 force, Vector2 pos)
	{

		eDamage.makeBodyBulletHole();
		base.Shot (headshot, damage,force,pos);

		if (died) {
			Death ();
		} else {
			droneCS.GetHit (force * damage);
		}
	}
	public override void BlownUp(bool headshot, float damage,Vector3 ePos)
	{
		
		if(!eDamage.blownUp){//if not just blownup prior
			Instantiate(hitEffect, transform.position, Quaternion.identity);
			base.BlownUp(headshot,damage,ePos);
			if (died) {
				Death();
			} else {
				var force = GetBlastDirection(transform.position,ePos);
				droneCS.GetHit (force * damage);
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
			} else {
				var force = GetBlastDirection(transform.position,ePos);
				droneCS.GetHit (force * damage);
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
		GetComponent<Rigidbody2D> ().gravityScale = 1;
		GetComponent<Animator>().Play ("DickBat_Death");
		Destroy (droneCS);
		Destroy (GetComponent<BoxCollider2D>());
		//if(transform.Find ("dWeapon")){
			//transform.Find ("dWeapon").GetComponent<SniperSight>().enabled = false;
		//}
		Destroy (gameObject,1.5f);
	}
	public override bool HurtPlayer(Vector3 pos)
	{
		if (canAttack) {
						canAttack = false;
						FlipTowardsPlayer ();
						pos.y += 2f;
						Vector3 charPos = transform.position;
			
						float diffX = pos.x - charPos.x;
						float diffY = pos.y - charPos.y;
			
						float angle2 = Mathf.Atan2 (diffY, diffX) * Mathf.Rad2Deg;
						//print (angle2);
						if (angle2 < -40f) {
								angle2 = -40f;
						}
						Quaternion angle3 = Quaternion.Euler (new Vector3 (0, 0, angle2));
						if (!facingRight) {
								angle2 = Mathf.Atan2 (diffY, diffX * -1) * Mathf.Rad2Deg;
								angle3 = Quaternion.Euler (new Vector3 (0, 0, angle2));
								if (angle2 < -90f) {
										angle2 = -90f;
								}
						}
						dickT.rotation = angle3;
						dickA.Play ("enemyShootGun");
			
						AudioSource.PlayClipAtPoint(growlFX[UnityEngine.Random.Range (0, growlFX.Length)], transform.position);
			
						StartCoroutine ("StopHurtPlayer");
						return true;
				} else {
					return false;
				}
	}
	IEnumerator StopHurtPlayer()
	{
		yield return new WaitForSeconds(.5f);
		canAttack = true;
		dickT.rotation = Quaternion.identity;
		//headA.GetComponent<SpriteRenderer>().sprite = Soldier_Sprites.S.getHead(0);
	}
	private void Flip()
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
	public void FlipTowardsPlayer()//should he turn around and face projectile
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
