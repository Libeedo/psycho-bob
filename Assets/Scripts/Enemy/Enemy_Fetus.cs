using UnityEngine;
using System.Collections;

public class Enemy_Fetus : MonoBehaviour {
	//public GameObject enemy;
	private float HP = 1f;
	// Use this for initialization
	void Start () {

		transform.Find ("fetus").GetComponent<Animator>().Play ("fetusBullet");
		StartCoroutine(Expire());
	}
	public void Hurt(float damage)
	{
		HP-=damage;
		if(HP<=0f){
			StartCoroutine("Death");
		}
	}
	IEnumerator Expire()
	{
		yield return new WaitForSeconds(2f);

		StartCoroutine("Death");
		

	}
	IEnumerator Death()
	{
		gameObject.tag = "EnemyRDLimb";
		transform.Find ("fetus").GetComponent<Animator>().Play("fetusDie");
		yield return new WaitForSeconds(0.5f);

		transform.Find ("fetus").GetComponent<SpriteRenderer>().enabled = false;
		//yield return new WaitForSeconds(.5f);
		transform.Find ("fetus").transform.Find ("blood").GetComponent<ParticleSystem>().Emit(1300);
		yield return new WaitForSeconds(.2f);
		Destroy(gameObject);
	}

}
