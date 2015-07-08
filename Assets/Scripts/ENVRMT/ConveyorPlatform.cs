using UnityEngine;
using System.Collections;

public class ConveyorPlatform : MonoBehaviour 
{
	//public GameObject explosion;		// Prefab of explosion effect.
	public bool damageMode = false;

	public float bouncePower = 2500f;

	//private bool justBounced = false;





	
	void OnTriggerEnter2D (Collider2D col) 
	{
		//if(justBounced){return;}
		if(col.tag == "Player")
		{
			if(damageMode){
				col.GetComponent<PlayerHealth>().Die ();
			}else{
				//print ("bounce "+col.gameObject.name+"   "+col.gameObject.transform);
				// ... find the Enemy script and call the Hurt function.
				Vector3 aim = transform.Find ("bounceAim").transform.position;
				Vector3 vecc = (aim - transform.position).normalized * bouncePower * 10f;
				col.GetComponent<PlayerControl>().Bounce(new Vector2(vecc.x * 2f,vecc.y));
			}
			//audio.Play();
			//StartCoroutine(bounceBack());
		}else if(col.tag == "Enemy" || col.tag == "EnemyHead")
		{
			//Vector3 aim = transform.Find ("bounceAim").transform.position;
			//Vector3 vecc = (aim - transform.position).normalized * bouncePower;
			col.transform.root.GetComponent<Enemy>().BlownUp(false,100f,transform.position);
			//col.transform.root.GetComponent<Enemy>().Bounce (new Vector2(vecc.x * 2f,vecc.y));
			//GetComponent<Animator>().SetTrigger("bounce");
			//StartCoroutine(bounceBack());
		}
	
		else if(col.tag == "Explosive")
		{
			if(col.transform.GetComponent<Rigidbody2D>()){
				Vector3 aim = transform.Find ("bounceAim").transform.position;
				Vector3 vecc = (aim - transform.position).normalized * bouncePower;
				col.GetComponent<Explosive>().Hurt(100,new Vector2(vecc.x * 2f,vecc.y),transform.position);
				//GetComponent<Animator>().SetTrigger("bounce");
			}
		}
	}
	/*IEnumerator bounceBack(){
		justBounced = true;
		yield return new WaitForSeconds(0.1f);
		justBounced = false;
	}*/
}
