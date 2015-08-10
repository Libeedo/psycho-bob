using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour
{

	public GameObject explosion;			// Prefab of explosion effect.
	public GameObject fire;
	public int HP = 15;
	private bool onFire = false;
	public int onFireHP = 0;//number of HP left before fire starts
	private bool dead = false;

	public bool enemyExplosion = false;

	public enum XplodeMode{
		NIL,
		ENEMY,//used only on enemy soldiers c4,  not ragdoll (soldier switches to ragdoll, ragdolls c4 also explodes! faaak
		CHUTE,
	}
	public XplodeMode xMode = XplodeMode.NIL;



	public GameObject deadObject;
	[HideInInspector]
	public Parachute chuteCS;

//	private float fuseTime = 6f;
	private float fireCount = 0f;
	private float fireExplodeDelay = 10f;
	public AudioClip[] soundFX;




	public void Hurt(int damage,Vector2 vel,Vector2 pos)
	{
		//print ("damage "+dead);
		//StartCoroutine(BombDetonation());
		HP -= damage;
		fireCount += damage;
		if(GetComponent<Rigidbody2D>()){
			GetComponent<Rigidbody2D>().AddForce(new Vector2(vel.x*30f,vel.y*30f));
			float tork = vel.x;
			if(pos.y < transform.position.y){
				tork = -tork;
			}
			GetComponent<Rigidbody2D>().AddTorque(tork*30f);
		}

		AudioSource.PlayClipAtPoint(soundFX[Random.Range(0,soundFX.Length)],transform.position);

		if(HP <= 0 && !dead){
			// ... call the death function.
			Explode ();
			return;
		}
		if(onFire){
			if(fireCount>fireExplodeDelay){
				Explode();
			}
		}else if (damage > 0 && HP <= onFireHP) {
			onFire = true;

			GameObject fire1 =  Instantiate(fire,transform.position, Quaternion.identity) as GameObject;
			fire1.transform.parent = gameObject.transform;
			InvokeRepeating("DelayExplode",0.5f,1f);
		}

	}


	private void DelayExplode()
	{
		fireCount++;
		if(fireCount>fireExplodeDelay){
				CancelInvoke();
				Explode();
			}
		
	}

	public void Explode()
	{
		//print ("explode");
		Destroy(GetComponent<Collider2D>());
		if (xMode == XplodeMode.ENEMY){
			transform.root.GetComponent<Enemy>().equipped = Enemy.Equipped.NOTHING;
		}else if (xMode == XplodeMode.CHUTE) {
			//print ("WTF "+transform.name+"  "+transform.position);
			chuteCS.closeChute();
		}


		if(deadObject){
			GameObject d =  Instantiate(deadObject,transform.position, Quaternion.identity) as GameObject;
			Destroy(d,1f);
		}
		//print ("bomb explicit");

		Destroy (gameObject);
		this.enabled = false;
		GameObject e = (GameObject)Instantiate(explosion,transform.position, Quaternion.identity);//Quaternion.identity);//new Vector3(transform.position.x,transform.position.y+6f,transform.position.z)
		if(enemyExplosion){	
			e.transform.Find("collider").GetComponent<Explosion>().enemyExplosion = true;
		}

	}
}
