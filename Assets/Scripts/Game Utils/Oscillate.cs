using UnityEngine;
using System.Collections;

public class Oscillate : MonoBehaviour {
	public float maxRot;
	//public float minRot;
	public float speed;
	//private bool dir = true;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Quaternion newRotation = Quaternion.AngleAxis(90, Vector3.right);
		transform.rotation= Quaternion.AngleAxis(Mathf.PingPong(Time.time * speed, maxRot)-40, Vector3.back);
		//transform.localRotation += speed;
		//if (transform.localRotation > maxRot || transform.localRotation < minRot) {
		//	speed = -speed;
		//}
	}
}
