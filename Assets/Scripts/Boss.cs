using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	public float HP = 50f;
	//private float maxHP;// How many times the enemy can be hit before it dies.
	
	
	
	protected bool dead = false;
	//bool died = false; //used to check if just died.


	public virtual void Hurt(float dmg, bool explosion)
	{
		takeDamage(dmg);
	}

	void takeDamage (float dmg) //returns ture if dead;
	{
		//dmg*=10f;
		HP -= dmg;
		//print ("hurt   "+HP);
		if(!dead){
			Level.instance.makeHitNum((Vector2)transform.position+Vector2.up*3,dmg);
		
			if(HP <= 0f){
				dead = true;
			}
			
		}
		
	}

}
