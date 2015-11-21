using UnityEngine;
using System.Collections;

public class TriggerDebris : MonoBehaviour
{




//	private float[] forceX = new float[] { -200f, 200f, 200f , 170f, -170f ,-200f};
	//private float[] forceY = new float[] { 200f, 200f, -30f, 200f,-100f, 200f};
	public float delay;
	public AudioClip SFX;
	public void Break()
	{
	
		foreach (Transform child in transform) {
			//child.parent = null;
			//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			//child.transform.rotation = randomRotation;
			if(child.gameObject.GetComponent<Rigidbody2D>()){
				child.gameObject.SetActive(true);
				child.GetComponent<DebrisPiece>().Death ();
			}
		}
		AudioSource.PlayClipAtPoint(SFX, transform.position);
		Destroy(gameObject,4f);
	}




}
