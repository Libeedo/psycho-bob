using UnityEngine;
using System.Collections;

public class Parachute : MonoBehaviour {
	public bool chuteDead = false;
	private bool chuteClosing = false;

	public AudioClip hurtFX;
	public Enemy enemyCS;
	// Use this for initialization
	//public Transform enemy;
	public enum Attached
	{
		ENEMY,
		PROP,

	}
	public Attached attached = Attached.ENEMY;



	public void Hurt(bool fire)
	{
		if(chuteDead){
			return;
		}
		if(fire)
		{
			transform.Find ("flame").GetComponent<ParticleSystem>().Play ();
		
		}
		GetComponent<AudioSource>().clip = hurtFX;
		GetComponent<AudioSource>().Play();
		//print ("chute hurt");
		chuteDead = true;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<Animator>().SetTrigger("dead");
		transform.parent.Find ("harness").transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
		//transform.root.Find ("harness").transform.rigidbody2D.mass = 0f;
		GetComponent<Rigidbody2D>().gravityScale = 0f;
		//rigidbody2D.mass = 0f;

		transform.GetComponent<HitColour>().Hurt(0.3f);
		StartCoroutine(CloseChuteDelay());

		switch(attached)
		{
		case Attached.ENEMY:
			if(enemyCS){
				enemyCS.chuteHurt(fire);
			}
			break;
		//case Attached.PROP:
			//enemy.transform.GetComponent<Enemy_Ragdoll>().chuteHurt(fire);
			//break;
		}
	}

	public void closeChute()
	{
		//return;
		if(chuteClosing){
			return;
		}
		chuteClosing = true;
		chuteDead = true;
		//collider2D.enabled = false;
		//rigidbody2D.gravityScale = 0.7f;
		//transform.GetComponent<Animator>().SetTrigger("dead");
		DistanceJoint2D[] jnts = transform.parent.Find ("harness").transform.GetComponents<DistanceJoint2D> ();
		foreach (DistanceJoint2D jnt in jnts) {

			Destroy (jnt);
		}
		Destroy(transform.parent.gameObject);
	}
	IEnumerator CloseChuteDelay()
	{
		yield return new WaitForSeconds(3.0f);
		closeChute();
	}

}
