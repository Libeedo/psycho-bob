using UnityEngine;
using System.Collections;

public class Deflective : MonoBehaviour
{

	public bool pushObjectBack = false; //true means a deflected bullet bounces the root object in the direction of the bullet that hit 
	public Rigidbody2D pushObjectRigidBody; //which master objects rigidbody2d should i push if above is true;
	public AudioClip[] soundFX;
	public void Deflect(Vector2 vel)
	{
		GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
		GetComponent<AudioSource>().Play ();
		if(pushObjectBack){
			pushObjectRigidBody.AddForce(vel);
		}
		
	}


}
