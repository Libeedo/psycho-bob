using UnityEngine;
using System.Collections;

public class ImpactSound : MonoBehaviour {
	public AudioClip[] soundFX;

	
	void OnCollisionEnter2D (Collision2D col)
	{
		float speed = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
	//	print (speed);
		if (speed > 2f) {
			GetComponent<AudioSource>().pitch = Random.Range(0f,1.005f);
			GetComponent<AudioSource>().volume = speed/20f;
			GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
			GetComponent<AudioSource>().Play ();
		}
	}
}
