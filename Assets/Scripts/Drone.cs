using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour {
	private bool moving = false;
	private Transform playerT;
	//private bool facingRight = true;
	private float rndX;
	private float rndY;
	public bool batMode = false;
	private Enemy_Dickbat batCS;
	private Rigidbody2D rb2D;
	void Start () {
		rb2D = GetComponent<Rigidbody2D> ();

		StartCoroutine("LateStart");
		rndX = Random.Range(-5,5);
		rndY = Random.Range(-5,5);
	}
	IEnumerator LateStart()
	{
		yield return new WaitForSeconds(.1f);
		playerT = Level.instance.GetPlayerTransform();
		if (batMode) {
			batCS = GetComponent<Enemy_Dickbat>();
		}
		moving = true;
		InvokeRepeating("StopMoving",0,1);
	}
	// Update is called once per frame
	void FixedUpdate () {
		//print (moving);
		if(moving){

			Vector3 moveTo = new Vector3(playerT.position.x+rndX,playerT.position.y+rndY,transform.position.z);
			/*var x = Random.Range(-1,1);
			var y = Random.Range(-1,1);
			moveTo.x += x;
			moveTo.y += y;*/
			transform.position = Vector3.MoveTowards(transform.position,moveTo,Time.deltaTime*10);
		}
	}

	void StopMoving()
	{
		//yield return new WaitForSeconds(1f);
		moving = !moving;
		rndX = Random.Range(-5,5);
		rndY = Random.Range(-5,5);
		if (batMode) {
			batCS.FlipTowardsPlayer();
		}
		/*if(facingRight){
			if(playerT.position.x < transform.position.x){
				Flip ();
			}
		}else if(playerT.position.x > transform.position.x){
			Flip ();
		}*/
	}

	public void GetHit(Vector2 vel)
	{
		moving = false;
		rb2D.isKinematic = false;
		rb2D.AddForce (vel);
		CancelInvoke ("StopMoving");
		StopCoroutine ("StopGetHit");
		StartCoroutine ("StopGetHit");
	}
	IEnumerator StopGetHit()
	{
		yield return new WaitForSeconds (0.5f);
		rb2D.isKinematic = true;
		InvokeRepeating("StopMoving",0,1);
	}
	/*private void Flip()
	{
		//return;
		//print (" enemy flip");
		facingRight = !facingRight;
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}*/
}
