using UnityEngine;
using System.Collections;

public class CameraZone : MonoBehaviour {
	public float zoomAmount = 0f;
	void OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
			Camera.main.GetComponent<CameraFollow>().levelZoom = zoomAmount;

		}
	}
}
