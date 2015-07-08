using UnityEngine;
using System.Collections;

public class DynamicTerrainPiece : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float speed = Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs (GetComponent<Rigidbody2D>().velocity.y);
		//print (speed);
		if (speed < 0.05) {
			GetComponent<Rigidbody2D>().isKinematic = true;
			//gameObject.layer = 16;
			Destroy(this);
		}
	}
}
