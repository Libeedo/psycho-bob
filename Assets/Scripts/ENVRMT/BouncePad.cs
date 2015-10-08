using UnityEngine;
using System.Collections;

public class BouncePad : MonoBehaviour 
{
	//public GameObject explosion;		// Prefab of explosion effect.

	public float bouncePower = 1200f;
	private bool justBounced = false;




	
	void OnTriggerEnter2D (Collider2D col) 
	{
		if(justBounced){return;}
		if(col.tag == "Player")
		{
			//print ("bounce "+col.gameObject.name+"   "+col.gameObject.transform);
			// ... find the Enemy script and call the Hurt function.
			Vector3 aim = transform.Find ("bounceAim").transform.position;
			Vector3 vecc = (aim - transform.position).normalized * bouncePower * 10f;
			if(col.GetComponent<PlayerControl>().Bounce(new Vector2(vecc.x * 2f,vecc.y))){
				GetComponent<Animator>().SetTrigger("bounce");
				GetComponent<AudioSource>().Play();
				StartCoroutine(bounceBack());
			}
		}
		else if(col.tag == "Nurse")
		{
			//print ("bounce "+col.gameObject.name+"   "+col.gameObject.transform);
			// ... find the Enemy script and call the Hurt function.
			Vector3 aim = transform.Find ("bounceAim").transform.position;
			Vector3 vecc = (aim - transform.position).normalized * bouncePower * 40f;
			col.GetComponent<Nurse>().Bounce(new Vector2(vecc.x * 2f,vecc.y));
			GetComponent<Animator>().SetTrigger("bounce");
			GetComponent<AudioSource>().Play();
			StartCoroutine(bounceBack());
		}
		else if(col.tag == "Enemy" ||col.tag == "EnemyHead")
		{
			Vector3 aim = transform.Find ("bounceAim").transform.position;
			Vector3 vecc = (aim - transform.position).normalized * bouncePower;
			col.transform.root.GetComponent<Enemy>().Bounce(vecc);
			GetComponent<Animator>().SetTrigger("bounce");
			//StartCoroutine(bounceBack());
		}
		/*else if(col.tag == "EnemyRD")
		{
			Vector3 aim = transform.Find ("bounceAim").transform.position;
			Vector3 vecc = (aim - transform.position).normalized * bouncePower;

			col.transform.root.GetComponent<Enemy_Master>().Bounce(new Vector2(vecc.x * 2f,vecc.y));
			GetComponent<Animator>().SetTrigger("bounce");
			//StartCoroutine(bounceBack());
		
		}*/
		else if(col.tag == "Explosive")
		{
			if(col.transform.GetComponent<Rigidbody2D>()){
				Vector3 aim = transform.Find ("bounceAim").transform.position;
				Vector3 vecc = (aim - transform.position).normalized * bouncePower;
				col.transform.GetComponent<Rigidbody2D>().AddForce (new Vector2(vecc.x * 2f,vecc.y));
				GetComponent<Animator>().SetTrigger("bounce");
			}
		}
	}
	IEnumerator bounceBack(){
		justBounced = true;
		yield return new WaitForSeconds(1f);
		justBounced = false;
	}
}
