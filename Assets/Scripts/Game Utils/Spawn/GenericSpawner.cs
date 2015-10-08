using UnityEngine;
using System.Collections;

public class GenericSpawner : Spawner {
	

	
	public GameObject[] objectsToSpawn;
	//public bool respawnDead
	
	public override void Spawn ()
	{
		
		base.Spawn ();
		if(canSpawn){
			int enemyIndex = Random.Range(0, objectsToSpawn.Length);
			GameObject go = (GameObject)Instantiate(objectsToSpawn[enemyIndex], transform.position, transform.rotation);
			

			if(respawnDead){
				var rfs = go.AddComponent<RemoveFromSpawner>();
				rfs.spawner = this;
			}
			
		
		}
	}
	
}
