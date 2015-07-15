using UnityEngine;
using System.Collections;

public class DebrisPiece : MonoBehaviour {

	public int HP = 1;					// How many times the enemy can be hit before it dies.
	private bool dead = false;
	public bool glassMode = false;  // for debrisObject to set if glass

	public void Hurt(int damage)
	{
		if(dead){return;}
		HP -= damage;
		
		if(HP <= 0 && !dead){
			// ... call the death function.
			dead = true;
			if(glassMode){
				transform.parent.GetComponent<DebrisObject>().Death();//kills all other pieces in SAME OBJECT as well.
			}else{//kills just this piece
				Death ();
			}
			//print ("hut3 "+HP);
		}
	}

	public void Death()
	{
		transform.parent = null;
		GetComponent<Rigidbody2D>().isKinematic = false;
		float r1 = Random.Range (-500f,500f);
		float r2 = Random.Range (200f,400f);
		GetComponent<Rigidbody2D>().AddForce(new Vector2(r1,r2));
		GetComponent<Rigidbody2D>().AddTorque(Random.Range(-200f,200f));
		Destroy (gameObject,4f);
		gameObject.tag = "Untagged";
		gameObject.layer = LayerMask.NameToLayer("Debris");
	}

	
	
	
	
	
}
