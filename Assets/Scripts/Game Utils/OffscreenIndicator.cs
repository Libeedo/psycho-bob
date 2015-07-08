using UnityEngine;
using System.Collections;

public class OffscreenIndicator : MonoBehaviour {
	private GameObject goToTrack;
	//private SpriteRenderer renderr;
	// Use this for initialization
	void Start () {
		goToTrack = GameObject.FindGameObjectWithTag("NursePoint").gameObject;
		//renderr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//print ("fuck");
		Vector3 v3Screen = Camera.main.WorldToScreenPoint(goToTrack.transform.position);
		float diffX = v3Screen.x - transform.position.x;
		float diffY = v3Screen.y - transform.position.y;
		
		float angle2 = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
		//Quaternion angle3 = 
		//Quaternion headAngle = angle3;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle2));
			
	/*	Vector3 v3Screen = Camera.main.WorldToViewportPoint(goToTrack.transform.position);
		//if (v3Screen.x > -0.01f && v3Screen.x < 1.01f && v3Screen.y > -0.01f && v3Screen.y < 1.01f){
			//renderer.enabled = false;
		//}else
		//{
			//renderer.enabled = true;
			v3Screen.x = Mathf.Clamp (v3Screen.x, 0.05f, 0.95f);
			v3Screen.y = Mathf.Clamp (v3Screen.y, 0.05f, 0.95f);
			v3Screen = Camera.main.ViewportToWorldPoint (v3Screen);
			//v3Screen.z = -2;
			transform.localPosition = v3Screen;
		//}
				
		*/	

	}
}
