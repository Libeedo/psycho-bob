using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {
	public int damage;
	private float scaleY;
	void Start () {
		Destroy (gameObject,1);
		scaleY = (float)damage/100;
		//print (scaleY+"  "+damage);
		transform.Find ("musicNotes").parent = null;
	}
	void FixedUpdate()
	{
		Vector2 scl = transform.localScale;
		scl.y += scaleY;
		transform.localScale = scl;

	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.tag+"  exploxed "  +col.gameObject.name);
		
		if(col.tag == "Enemy"){

				//print ("flame enemy");
				col.transform.root.GetComponent<Enemy>().Flamed();
				//Vector3 force =	getBlastDiretion(col.transform.position);
				col.transform.root.GetComponent<Enemy>().BlownUp (false,damage,transform.position);
				

		}
		else if(col.tag == "EnemyHead")
		{
			//print ("flame head");
				col.transform.root.GetComponent<Enemy>().Flamed();
				//Vector3 force =	getBlastDiretion(new Vector3(col.transform.position.x,col.transform.position.y+3f,transform.position.z));
				col.transform.root.GetComponent<Enemy>().BlownUp (true,damage,transform.position);
				

		}

		else if(col.tag == "EnemyFetus"){
			col.transform.GetComponent<Enemy_Fetus>().Hurt (damage);
		}

		else if(col.tag == "Parachute")
		{
			
			col.GetComponent<Parachute>().Hurt (true);
			
		}
		else if(col.tag == "Explosive")
		{
			Vector3 deltaPos = col.transform.position - transform.position;
			
			// Apply a force in this direction with a magnitude of bombForce.
			Vector3 force = deltaPos.normalized * damage;
			// ... find the Bomb script and call the Explode function.
			col.GetComponent<Explosive>().Hurt(80,force,transform.position);
			
		}
		else if(col.tag == "HitZone")
		{
			
			col.GetComponent<HitZone>().hitDel(damage,true);
			
		}
		else if(col.tag == "Debris")
		{
			
			col.GetComponent<DebrisPiece>().Hurt(80);
			
		}

	}

}
