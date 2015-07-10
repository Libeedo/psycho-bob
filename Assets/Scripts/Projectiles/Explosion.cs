using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	//private float bombRadius = 100f;			// Radius within which enemies are killed.
	public float bombForce = 500f;			// Force that enemies are thrown from the blast.
	//private float maxBombForce = 100f; //if changed,  change soldiers blownup;
	public AudioClip boom;					// Audioclip of explosion.
	//public AudioClip fuse;					// Audioclip of fuse.
			// Prefab of explosion effect.

	public bool enemyExplosion = false;
	//private float growForce = 1.06f;
	//private PickupSpawner pickupSpawner;	// Reference to the PickupSpawner script.
	//private ParticleSystem explosionFX;		// Reference to the particle system of the explosion effect.

	//private int layerMsk;
	void Awake ()
	{
		//bombForce = maxBombForce;

		Destroy (transform.root.gameObject, 1f);
	}

	void Start ()
	{
		print ("explosion!!!!!!!!");
		AudioSource.PlayClipAtPoint(boom, transform.position);
		//InvokeRepeating("Explode",0f,0.02f);
		if(enemyExplosion){
			transform.root.Find ("explosionFireball").Find ("fireBall").gameObject.SetActive(false);
			transform.root.Find ("explosionFireball").Find ("fireBall blue").gameObject.SetActive(true);
		}
	}





	/*void Explode()
	{
		Vector2 scl = transform.localScale;
		scl.x *= growForce;
		scl.y *= growForce;
		transform.localScale = scl;

		growForce -= 0.001f;
		bombForce -= 1.3f;
		//print (bombForce);
		// Destroy the bomb.
		//Destroy (gameObject);
	}*/
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.tag+"  exploxed "  +col.gameObject.name);
		if (enemyExplosion) {
			if(col.tag == "Player" )
			{

				col.GetComponent<PlayerHealth>().Hurt(transform);

				Vector3 deltaPos = col.transform.position - transform.position;
				// Apply a force in this direction with a magnitude of bombForce.
				Vector3 force = deltaPos.normalized * bombForce;// / 50f;
				// ... find the Bomb script and call the Explode function.
				col.GetComponent<Rigidbody2D>().AddForce(force);
				
			}
			return;
		}
		if(col.tag == "Enemy" || col.tag == "EnemyHead"){
			// Find the Enemy script and set the enemy's health to zero.
			//col.gameObject.GetComponent<Enemy>().HP = 0;
			float distance = Vector2.Distance(col.transform.position , transform.position);
			//print ("distance "+distance);
			//if(damage > 0){
				/*if(enemyExplosion){
					col.transform.root.GetComponent<Enemy>().eDamage.scoreDeath = false;
				}*/
				col.transform.root.GetComponent<Enemy>().Flamed();
				//Vector3 force =	getBlastDiretion(new Vector3(col.transform.position.x,col.transform.position.y+3f,transform.position.z),90f);
				col.transform.root.GetComponent<Enemy>().BlownUp (false,15-distance, transform.position);//new Vector2(force.x,force.y));

			//}
		}

		else if(col.tag == "Parachute")
		{

			col.GetComponent<Parachute>().Hurt (true);

		}
		else if(col.tag == "Explosive")
		{
			Vector3 deltaPos = col.transform.position - transform.position;
			
			// Apply a force in this direction with a magnitude of bombForce.
			Vector3 force = deltaPos.normalized * bombForce;
			// ... find the Bomb script and call the Explode function.
			col.GetComponent<Explosive>().Hurt(80,force,transform.position);
			
		}
		else if(col.tag == "HitZone")// && !enemyExplosion)
		{
			float distance = Vector2.Distance(col.transform.position , transform.position);
			col.GetComponent<HitZone>().hitDel(60-distance,true);

			
		}
		else if(col.tag == "Debris")
		{
			
			col.GetComponent<DebrisPiece>().Hurt(80);
			
		}
		else if(col.tag == "Player" )
		{

			Vector3 deltaPos = col.transform.position - transform.position;
			// Apply a force in this direction with a magnitude of bombForce.
			Vector3 force = deltaPos.normalized * bombForce / 50;
			// ... find the Bomb script and call the Explode function.
			col.GetComponent<Rigidbody2D>().AddForce(force);
			
		}
	}

	/*Vector3 getBlastDiretion(Vector3 colPos,float multiplier)
	{
		// Find a vector from the bomb to the enemy.

		Vector3 deltaPos = colPos - transform.position;
		
		// Apply a force in this direction with a magnitude of bombForce.
		Vector3 force = deltaPos.normalized * (bombForce  * multiplier);
		Debug.DrawLine(deltaPos.normalized,transform.position, Color.red);
		return force;
		//col.AddForce(force);
		//print ("hit enemy");

	}*/
}
