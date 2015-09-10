
using UnityEngine;
using System.Collections;

public class KickHit : MonoBehaviour {
	//public int damage = 1;

	private float bouncePower = 5;
	//private float bounceDirX;
	//private Vector3 bouncePos;



	public void KickStart()
	{
		bouncePower = 4.5f;
		InvokeRepeating("OnHit",0,.3f);
		//bounceDirX = dir;
		//print (bounceDirX);
	}
	
	void OnHit()
	{
		// Create a quaternion with a random rotation in the z-axis.


		bouncePower -= 0.5f;
		if(bouncePower<0){
			bouncePower = 0.5f;
			CancelInvoke();
			//print ("stopped");
		}
		//var dir = transform.position;
		//print ("kh enemy "+bounceDirX+"  "+dir);
		//dir.x -= bounceDirX /10;
		//dir.y -=bounceDirX;
		//bouncePos = dir;
		//Destroy(gameObject);
		//return bouncePos;
	}

	void OnTriggerEnter2D (Collider2D col) 
	{

		//print (col.name);
		if(col.tag == "Enemy")
		{
			OnHit();
			//Vector3 vecc = (col.transform.position - transform.position).normalized * bouncePower;
			col.transform.root.GetComponent<Enemy>().Kicked(false,bouncePower,transform.position);// * bouncePower*200f);
			//col.transform.rigidbody2D.AddForce(transform.rigidbody2D.velocity * bouncePower);
			
			//GetComponent<Animator>().SetTrigger("bounce");

		}
		else if(col.tag == "EnemyHead")
		{
			OnHit();
			//Vector3 vecc = (col.transform.position - transform.position).normalized * bouncePower;
			col.transform.root.GetComponent<Enemy>().Kicked(true,bouncePower,transform.position);// * bouncePower*200f);
			//col.transform.rigidbody2D.AddForce(transform.rigidbody2D.velocity * bouncePower);
			
			//GetComponent<Animator>().SetTrigger("bounce");
			
		}
		/*else if(col.tag == "EnemyFetus")
		{
			OnHit();
			// ... find the Enemy script and call the Hurt function.
			col.GetComponent<Rigidbody2D>().AddForce(bouncePos);// * bouncePower*50f);
			//Destroy (col.gameObject);
			// Call the explosion instantiation.
			OnHit();

		}*/

		else if(col.tag == "Explosive")
		{
			//if(col.transform.rigidbody2D){
			OnHit();
				Vector3 vecc = (col.transform.position - transform.position).normalized * 70;
			col.GetComponent<Explosive>().Hurt(0,vecc,transform.position);// * bouncePower* 1.2f,transform.position);
			Level.instance.makeHitFX(col.transform.position);
			//OnHit();
		}
		else if(col.tag == "Parachute")
		{
			OnHit();
			col.gameObject.GetComponent<Parachute>().Hurt (false);
		
		}
		else if(col.tag == "Debris")
		{
			OnHit();
			col.GetComponent<DebrisPiece>().Hurt(1);
		


		}
		/*else if(col.tag == "Nurse")
		{
			//col.rigidbody2D.AddForce(transform.rigidbody2D.velocity * bouncePower*50f);
			OnHit(col.transform.position);
			col.GetComponent<Nurse>().Kicked(transform.position);// * bouncePower*70f);
			//Destroy (col.gameObject);
			// Call the explosion instantiation.
			//OnHit();
		}*/
		else if(col.tag == "HitZone")
		{
			OnHit();
			col.GetComponent<HitZone>().hitDel(1,false);

			
		}
	}
}
