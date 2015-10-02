using UnityEngine;
using System.Collections;

public class EnableTrigger : MonoBehaviour {
	public GameObject[] enableObjects;
	public GameObject[] disableObjects;
	//public bool enable = false;

	void OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
		
			foreach (GameObject go in enableObjects) {


					if(!go.activeSelf){
						//print (go.name);
						go.SetActive(true);
						if(go.GetComponent<Spawner>()){
							go.GetComponent<Spawner>().StartSpawn ();
						}
					}
			}
			foreach (GameObject go in disableObjects) {
					go.SetActive(false);
					if(go.GetComponent<Spawner>()){
						go.GetComponent<Spawner>().StopSpawn ();
					}//else{

						//.GetComponent<VehicleSpawner>().StopSpawn ();
					//}



			}

			Destroy(gameObject);
		}
	}
}
