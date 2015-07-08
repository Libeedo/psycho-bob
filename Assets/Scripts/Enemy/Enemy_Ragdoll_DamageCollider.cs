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
	void Awake()
	{
		if(head){
			hurt+=1;
		} 
	}
	void OnCollisionEnter2D (Collision2D col) {
		//sreturn;
			float vel = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
			if (vel > 3f) {
				//audio.pitch = Random.Range(-.005f,1.005f);
				GetComponent<AudioSource>().volume = vel/20f;
				GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
				GetComponent<AudioSource>().Play ();
				
				if (vel > 15f) {
						float hurt2 = hurt;
						
						if(vel >30f){
							hurt2 += 3f;
						}else if(vel >25f){
							hurt2 += 2f;
						}else if(vel >20f){
							hurt2 += 1f;
						}
						
						//Debug.Log ("HeadHit " + vel+"  "+col.gameObject.name);
						transform.root.GetComponent<Enemy> ().Damaged (head,hurt2);

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
