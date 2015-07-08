using UnityEngine;
using System.Collections;

public class ImpactDamage : MonoBehaviour {

	void OnCollisionEnter2D (Collision2D col) {
		//return;
		float vel = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
		if (vel > 20f) {
			foreach (Transform child in transform.root) {
				print (child.name);
				if(child.name == "top"){
					Destroy(child.GetComponent<HingeJoint2D>());

				}
			}
					
		}
	}
}
