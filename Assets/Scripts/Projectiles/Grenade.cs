using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
						// Audioclip of explosion.
	//public AudioClip fuse;					// Audioclip of fuse.

	public GameObject explosion;			// Prefab of explosion effect.

	private Vector2  speed;


	void Start ()
	{
		
		// If the bomb has no parent, it has been laid by the player and should detonate.
		//if(transform.root == transform)
			StartCoroutine(Detonate());
	}
	void FixedUpdate()
	{
		speed = GetComponent<Rigidbody2D>().velocity;
	}
	void OnCollisionEnter2D (Collision2D col)
	{
		if(col.gameObject.tag == "Debris")//so that grenades can collide with windows and go through, maintaing speed. only way i could get it to collide and go thru
		{

			col.transform.GetComponent<DebrisPiece>().Hurt(1);
			//Physics2D.IgnoreCollision(col.gameObject.collider2D,gameObject.collider2D);
			//print (rigidbody2D.velocity+"  "+speed);
			GetComponent<Rigidbody2D>().velocity = speed;
		}
	}
	IEnumerator Detonate()
	{

		// Play the fuse audioclip.
		//AudioSource.PlayClipAtPoint(fuse, transform.position);

		// Wait for 2 seconds.
		yield return new WaitForSeconds(2f);

		// Explode the bomb.
		Explode();
	}


	public void Explode()
	{
		//print ("bomb explicit");
		Instantiate(explosion,transform.position, Quaternion.identity);//Quaternion.Euler(new Vector3(0, 0.1f, 0)));
		Destroy (gameObject);


	}
}
