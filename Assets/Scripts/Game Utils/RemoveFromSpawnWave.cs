using UnityEngine;
using System.Collections;

public class RemoveFromSpawnWave : MonoBehaviour {

	public CombatZone combatZone;
	void OnDestroy()
	{
		combatZone.waveEnemies.Remove (gameObject);
		if (combatZone.waveEnemies.Count == 0) {
			combatZone.NextWave();
		}
	}
}
