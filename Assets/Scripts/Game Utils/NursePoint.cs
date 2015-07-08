using UnityEngine;
using System.Collections;

public class NursePoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter2D (Collider2D col)
	{
		print (col.name);
		if(col.tag == "Nurse")
		{
			Level.instance.WinLevel();
			Destroy (gameObject);
		}
	}
}
