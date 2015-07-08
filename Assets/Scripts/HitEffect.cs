using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour {
	public AudioClip[] kickSFX;

	// Use this for initialization
	void Start () {
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		
		transform.rotation = randomRotation;
		AudioSource.PlayClipAtPoint(kickSFX[UnityEngine.Random.Range (0, kickSFX.Length)], transform.position);
		Destroy (gameObject,.5f);
	}
	

}
