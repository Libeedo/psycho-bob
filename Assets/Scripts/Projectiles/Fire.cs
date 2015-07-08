using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	int layerMsk;
	void Start () {

		//InvokeRepeating("Burn",0f,0.25f);
		Destroy (gameObject,1.6f);
	}
	//void Burn ()
	void OnTriggerEnter2D (Collider2D col) 
	{
		//bool colTrue = false;
		//col  =  Physics2D.Linecast (transform.position,new Vector2(transform.position.x+2f,transform.position.y), layerMsk);//new Vector2(transform.position.x+2f,transform.position.y), new Vector2(
		//if(col){// && col.transform.gameObject.tag == "Enemy"){
			//print ("flamage "+ col.tag);
		//print ("flame "+col.name);
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
