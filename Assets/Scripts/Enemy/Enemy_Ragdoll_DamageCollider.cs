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
	public bool fallActivated = false; //falling
	public float fallTime = 0;//time when started falling


	void Awake()
	{
		if(head){
			hurt+=1;
		} 
	}
	void OnCollisionEnter2D (Collision2D col) {

			float vel = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
			if (vel > 3f) {
				if(fallActivated){
					print (Time.time - fallTime+"   "+vel);
					fallActivated = false;
				}
				//audio.pitch = Random.Range(-.005f,1.005f);
				GetComponent<AudioSource>().volume = vel/10f;
				GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
				GetComponent<AudioSource>().Play ();
				
				if (vel > 5f) {
						float hurt2 = hurt;
						
						if(vel >20f){
							hurt2 += 3f;
						}else if(vel >15f){
							hurt2 += 2f;
						}else if(vel >10f){
							hurt2 += 1f;
						}
						
						//Debug.Log ("HeadHit " + vel+"  "+col.gameObject.name);
						transform.root.GetComponent<Enemy> ().Damaged (head,hurt2,Vector2.zero);

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
