using UnityEngine;
using System.Collections;

public class SaddleTrigger : MonoBehaviour {


	void OnTriggerEnter2D (Collider2D col) 
	{
		
		//		print (col.name);
		if(col.tag == "Player")
		{

			Nurse nurseCS = transform.parent.GetComponent<Nurse>();
			nurseCS.enabled = true;
			nurseCS.Mounted (col.transform);

			col.GetComponent<PlayerControl>().SaddleUp(transform.parent.GetComponent<Rigidbody2D>(),nurseCS.facingRight);
			//col.GetComponent<PlayerControl>().enabled = false;
			//transform.parent.parent = col.transform;
		}

	}
}
