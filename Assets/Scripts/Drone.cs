using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour {
	private bool moving = false;
	private Transform playerT;
	private bool facingRight = true;
	private float rndX;
	private float rndY;
	void Start () {

		StartCoroutine("LateStart");
		rndX = Random.Range(-5,5);
		rndY = Random.Range(-5,5);
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
		if(facingRight){
			if(playerT.position.x < transform.position.x){
				Flip ();
			}
		}else if(playerT.position.x > transform.position.x){
			Flip ();
		}
	}
	IEnumerator LateStart()
	{
		yield return new WaitForSeconds(.1f);

		playerT = Level.instance.GetPlayerTransform();
		//print (playerT);
		//yield return new WaitForSeconds(.5f);
		moving = true;
		InvokeRepeating("StopMoving",0,1);
	}
	private void Flip()
	{
		//return;
		//print (" enemy flip");
		facingRight = !facingRight;
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}
}
