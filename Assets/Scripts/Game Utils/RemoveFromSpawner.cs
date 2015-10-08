using UnityEngine;
using System.Collections;

public class RemoveFromSpawner : MonoBehaviour {

	public Spawner spawner;
	void OnDestroy()
	{
		spawner.subtractSpawnCount();
	}
}
