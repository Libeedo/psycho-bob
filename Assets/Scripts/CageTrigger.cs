using UnityEngine;
using System.Collections;

public class CageTrigger : MonoBehaviour {
	private int bodyCount = 0;
	public int maxCount = 5;
	public GameObject targetObject;
	void OnTriggerEnter2D (Collider2D col) 
	{
		if (col.tag == "Enemy") {
			// ... find the Enemy script and call the Hurt function.
			if(col.transform.root.GetComponent<CageCount> ().AddToCage()){
				col.transform.root.GetComponent<Enemy>().Damaged(false,1000);
				bodyCount++;
				print ("BODY COUNT "+bodyCount);
				if(bodyCount == maxCount){
					targetObject.GetComponent<Animator>().SetTrigger("switchON");
					Destroy (gameObject);
				}
			}

		}
	}
}
