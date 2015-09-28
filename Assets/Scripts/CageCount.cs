using UnityEngine;
using System.Collections;

public class CageCount : MonoBehaviour {

	private bool added = false;
	
	// Update is called once per frame
	public bool AddToCage () {
		if(!added){
			added = true;
			return added;
			//Destroy(this);
		}
		return false;
	}
}
