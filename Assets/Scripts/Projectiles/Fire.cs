using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public float destroyTime;
	void Start () {

		//InvokeRepeating("Burn",0f,0.25f);
		Destroy (gameObject,destroyTime);
	}
	//void Burn ()
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.gameObject.tag);
		if(col.tag == "Enemy" || col.tag == "EnemyHead"){
			col.transform.root.GetComponent<Enemy>().Flamed();
		}else if(col.tag == "Parachute"){
			col.GetComponent<Parachute>().Hurt (true);

		}else if(col.tag == "Explosive")
		{
			col.GetComponent<Explosive>().Hurt(20,Vector2.zero,transform.position);
		}
		else if(col.tag == "HitZone")
		{
			col.GetComponent<HitZone>().hitDel(1,false);
			
		}
		else if(col.tag == "EnemyFetus")
		{
			col.transform.GetComponent<Enemy_Fetus>().Hurt (1);
		}
		else if(col.tag == "Debris")
		{
			
			col.GetComponent<DebrisPiece>().Hurt(80);
			
		}
		//}
	}
	//void OnCollisionEnter2D ()
	//{
	//	print("flamage");
	//}
}
