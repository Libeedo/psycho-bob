using UnityEngine;
using System.Collections;

public class EnableSpawnPoint : MonoBehaviour {

	private SpawnPoint spawnPoint;
	void Start()
	{
		spawnPoint = transform.parent.GetComponent<SpawnPoint>();
	}
	void OnTriggerEnter2D (Collider2D col)
	{
		if(col.tag == "Player")
		{
			spawnPoint.EnableSpawn();
		}
	}
}
