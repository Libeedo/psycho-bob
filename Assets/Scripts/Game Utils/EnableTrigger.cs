using UnityEngine;
using System.Collections;

public class EnableTrigger : MonoBehaviour {
	public GameObject[] gObjects;
	public bool enable = false;

	void OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
		
			foreach (GameObject go in gObjects) {

				if(enable){
					if(!go.activeSelf){
						//print (go.name);
						go.SetActive(true);
						if(go.GetComponent<Spawner>()){
							go.GetComponent<Spawner>().StartSpawn ();
						}
					}
				}else{
					go.SetActive(false);
					if(go.GetComponent<Spawner>()){
						go.GetComponent<Spawner>().StopSpawn ();
					}//else{

						//.GetComponent<VehicleSpawner>().StopSpawn ();
					//}

				}

			}

			Destroy(gameObject);
		}
	}
}
