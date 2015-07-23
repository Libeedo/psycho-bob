using UnityEngine;
using UnityEditor;
using System.Collections;

public class CombatZone : MonoBehaviour {

	public SpawnWave spawnWave;
	public SpawnSequence spawnSequence;
	public SpawnEvent spawnEvent;
	public CameraLimits cameraLimits;

	public enum EnemyType{
		SOLDIER,
		DICKBUG,
		DICKBAT
	}

	//void Start () {
		//cameraLimits = new CameraLimits ();
	//}
	
	// Update is called once per frame
	//void Update () {
	
	//}
}
[System.Serializable]
public class SpawnWave
{
	public SpawnSequence[] spawnSeq;
}
[System.Serializable]
public class SpawnSequence
{
	public SpawnEvent[] spawnEvent;
}
[System.Serializable]
public class SpawnEvent
{
	public CombatZone.EnemyType enemyType;
	public Enemy.Equipped equipped;
	public Vector2 pos;
}
[System.Serializable]
public class CameraLimits
{
	public Vector2 max;
	public Vector2 min;
	public int zoom;
}