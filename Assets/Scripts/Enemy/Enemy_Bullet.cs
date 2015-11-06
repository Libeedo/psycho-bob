using UnityEngine;
using System.Collections;

public class Enemy_Bullet : MonoBehaviour {
	public Vector3 moveToPos;
	private float speed = 12f;
	public bool findPlayer = false;
	void Start () {
		if(findPlayer){
			Vector3 plPos = Level.instance.GetPlayerTransform().position;
			plPos.y += 2;
			moveToPos = ( plPos-transform.position).normalized * 50000f;
			//moveToPos =  plPos;//transform.position + (transform.right*80f);
			Destroy(gameObject,4f);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.MoveTowards(transform.position,moveToPos,Time.deltaTime*speed);
	}
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.tag);
		// If it hits an enemy...
		if (col.tag == "Player") {
			col.GetComponent<PlayerHealth>().Hurt(false,transform);
		}
		speed = 0;
		Destroy (gameObject,.4f);
		GetComponent<Animator>().Play ("enemyBulletHit");
	}
}
