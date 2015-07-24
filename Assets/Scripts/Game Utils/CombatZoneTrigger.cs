using UnityEngine;
using System.Collections;

public class CombatZoneTrigger : MonoBehaviour {
	private bool triggered = false;
	void OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
			if(triggered){return;}
			transform.parent.GetComponent<CombatZone>().StartZone();
			triggered = true;
			Destroy (gameObject);
			
		}
	}
}
