using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
	//public GameObject splash;
	//public GameObject heroDead;
	///private GameObject player;
	//private bool playerDead = false;
	void OnTriggerEnter2D(Collider2D col)
	{
		//print ("hit "+col.tag);
		//print ("hit2 "+col.gameObject.name);

		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
			//print ("kill player");
			//if(!playerDead){
				//player = col.gameObject;
				Level.instance.KillPlayer();//player);

			//}
		}
		else if(col.gameObject.tag == "Explosive")
		{
			//print ("explode");
			col.transform.GetComponent<Explosive>().Explode();
		}
		else if(col.gameObject.tag == "Enemy"  || col.gameObject.tag == "EnemyHead" )
		{
			//Enemy es = col.transform.root.GetComponent<Enemy>();
			//es.eDamage.scoreDeath = false;
			//es.closeChute();
			col.transform.root.GetComponent<Enemy>().Damaged ( false,float.MaxValue);
		}
		else if(col.gameObject.tag == "Vehicle")
		{
			col.transform.root.GetComponent<Vehicle>().Hurt(int.MaxValue,false);
		}
		else if(col.gameObject.tag == "Nurse")
		{

			col.GetComponent<Nurse>().Death ();
			Level.instance.KillPlayer();
		}
		/*else if(col.gameObject.tag != "EnemyRDLimb")
		{
			print ("hitttt "+col.tag);
			print ("hit2 "+col.gameObject.name);
			//print (col.gameObject.transform.root.gameObject.name);
			// ... instantiate the splash where the enemy falls in.
			Instantiate(splash, col.transform.position, transform.rotation);

			// Destroy the enemy.
			//Destroy (col.gameObject);	
			Destroy (col.gameObject.transform.root.gameObject);
		}*/
	}



}
