using UnityEngine;
using System.Collections;

public class VehicleImpactExplosion : MonoBehaviour {
	public Vehicle vehicleCS;
	void OnCollisionEnter2D (Collision2D col)
	{
		if(col.transform.tag =="ground"){
		//	print (col.gameObject.name+"   "+col.gameObject.tag);
			vehicleCS.Hurt(int.MaxValue,false);
			//Destroy (this);
		}else if(col.transform.tag == "Enemy"){
			print ("BEAT DOWN");
			Vector2 vel = vehicleCS.getSpeed();
			col.transform.root.GetComponent<Enemy>().Damaged (false,Mathf.Abs(vel.x)+Mathf.Abs(vel.y)*20f);//,vel*20f);
		}else if(col.transform.tag == "EnemyHead"){
			print ("BEAT DOWN");
			Vector2 vel = vehicleCS.getSpeed();
			col.transform.root.GetComponent<Enemy>().Damaged (true,Mathf.Abs(vel.x)+Mathf.Abs(vel.y)*20f);//,vel*20f);
		}
	}
}
