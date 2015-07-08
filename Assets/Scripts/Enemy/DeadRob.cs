using UnityEngine;
using System.Collections;

public class DeadRob : MonoBehaviour 
{
	//public GameObject explosion;		// Prefab of explosion effect.

	//private float[] forceX = new float[] { -400f, 400f, 400f , 350f, 0f,-350f,-350f ,-400f};
	//private float[] forceY = new float[] { 500f, 500f, -30f, 0f, 400f,-100f,0f, 600f};
	public int sortingNum;
	void Start () 
	{
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		//GameObject[] pieces = new GameObject[8];
		//transform.localScale = new Vector3(0.8f,0.8f,1);
	

		//Destroy(gameObject, 3);
		//deadRob0.parent = null;
		StartCoroutine(headExplode());
	}




	IEnumerator headExplode()
	{
		// Play the fuse audioclip.
		//AudioSource.PlayClipAtPoint(fuse, transform.position);
		
		// Wait for 2 seconds.
		yield return new WaitForSeconds(0.1f);
		
		// Explode the bomb.
			//int count = 0;
			Destroy(gameObject,4);
			foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>()) {
				//child.parent = null;
				//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
				//child.transform.rotation = randomRotation;
				//if(child.GetComponent<Rigidbody2D>()){
					//Rigidbody2D rb =  child.GetComponent<Rigidbody2D>();
					rb.AddTorque(Random.Range(-3600, 3600));
					rb.AddForce(new Vector2(Random.Range(-500, 500),Random.Range(-500, 500)));
					rb.GetComponent<SpriteRenderer>().sortingOrder += sortingNum;
					//rb.gravityScale = 0f;
					//Destroy(child.gameObject,3);
					//pieces[count] = child.gameObject;
					//count++;
				//}
			}
	}

	/*void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy")
		{
			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<Enemy>().Hurt(5);

			// Call the explosion instantiation.
			OnExplode();

			// Destroy the rocket.
			Destroy (gameObject);
		}
		// Otherwise if it hits a bomb crate...
		else if(col.tag == "BombPickup")
		{
			// ... find the Bomb script and call the Explode function.
			col.gameObject.GetComponent<Bomb>().Explode();

			// Destroy the bomb crate.
			Destroy (col.transform.root.gameObject);

			// Destroy the rocket.
			Destroy (gameObject);
		}
		// Otherwise if the player manages to shoot himself...
		else if(col.gameObject.tag != "Player")
		{
			// Instantiate the explosion and destroy the rocket.
			OnExplode();
			Destroy (gameObject);
		}
	}*/
}
