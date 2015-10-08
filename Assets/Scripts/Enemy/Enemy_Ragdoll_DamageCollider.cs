using UnityEngine;
using System.Collections;

public class Enemy_Ragdoll_DamageCollider : MonoBehaviour 
{

	//private Animator headAnim;
	public AudioClip[] soundFX;
	//private bool justHurt = false;
	//private string[] animArray = new string[]{"Enemy1_head_damage5","Enemy1_head_damage5","Enemy1_head_damage4","Enemy1_head_damage3","Enemy1_head_damage2","Enemy1_head_damage1"};
	private float hurt = 1f;
	public bool head = false;

	//for a better splat after a long fall
	//public bool fallActivated = false; //falling
	//public float fallTime = 0;//time when started falling

	private Enemy_Ragdoll rdCS;
	//private Enemy_Ragdoll_DamageCollider otherDColliderCS;
	void Awake()
	{
		if(head){
			//otherDColliderCS = transform.parent.Find ("enemy1_deadBody_body").GetComponent<Enemy_Ragdoll_DamageCollider>();
			hurt+=1;
		}
		rdCS = transform.parent.GetComponent<Enemy_Ragdoll>();
	}
	void OnCollisionEnter2D (Collision2D col) {

			float vel = rdCS.lastSpeed;//Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
			if (vel > 5f) {
				//if(fallActivated){
					//print (head+"     "+vel);
					//fallActivated = false;
					//otherDColliderCS.fallActivated = false;
				//}
				//audio.pitch = Random.Range(-.005f,1.005f);
				GetComponent<AudioSource>().volume = vel/40f;
				GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
				GetComponent<AudioSource>().Play ();
				
				if (vel > 15f) {
						float hurt2 = hurt;
						if(vel>40){
							rdCS.Damaged (head,100,Vector2.zero);
							rdCS.DestroyLimbs(Vector2.up*200);
						}else if(vel >30f){
							hurt2 += 3f;
						}else if(vel >20f){
							hurt2 += 2f;
						}else if(vel >20f){
							hurt2 += 1f;
						}
						
						//Debug.Log ("HeadHit " + vel+"  "+col.gameObject.name);
						rdCS.Damaged (head,hurt2,Vector2.zero);

						//justHurt = true;
					//if (gameObject.activeSelf) {
							//StartCoroutine (endJustHurt ());
					//
				}
			}
	}
	//IEnumerator endJustHurt()
	//{
		//yield return new WaitForSeconds(.7f);
		//justHurt = false;
	//}

}
