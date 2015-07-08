using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.
	public bool enemyExplosion = false;
	//private int damage = 10;
	public bool stayAlive = false;
	void Start () 
	{
		if(!stayAlive){
			Destroy(gameObject, 7);
		}
	}



	
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.transform.name);
		if(col.tag == "Parachute" && !enemyExplosion)
		{

			col.gameObject.GetComponent<Parachute>().Hurt (false);
			HingeJoint2D jnt = gameObject.AddComponent<HingeJoint2D>();
			jnt.connectedBody = col.gameObject.transform.GetComponent<Rigidbody2D>();
			jnt.anchor = new Vector2(3f,0);
			jnt.connectedAnchor = new Vector2(0,2.3f);
		}else{
			// Create a quaternion with a random rotation in the z-axis.
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			
			// Instantiate the explosion where the rocket is with the random rotation.
			//Instantiate(explosion, transform.position, randomRotation);
			GameObject x = (GameObject)Instantiate(explosion,transform.position, Quaternion.identity);//Quaternion.Euler(new Vector3(0, 0.1f, 0)));
			if(enemyExplosion){
				x.transform.Find ("collider").GetComponent<Explosion>().enemyExplosion = true;
			}
			if(stayAlive){//drone bomb

				Level.instance.score.UpdateScore(50,new Vector3(transform.position.x,transform.position.y+5f));

			}
			Destroy (gameObject);
		}
	}
}
