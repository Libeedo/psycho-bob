using UnityEngine;
using UnityEditor;
using System.Collections;

public class CombatZone : MonoBehaviour {

	public GameObject[] enemies;

	public CameraLimits cameraLimits;
	public float delayNextWave;
	public Wave[] spawnWaves;


	//private SpawnWave currentWave;
	//public SpawnSequence spawnSequence;
	//public SpawnEvent spawnEvent;

	int waveCount = 0;
	int seqCount = 0;
	int eventCount = 0;

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
	public void StartZone()
	{
		print("startZone");
		Camera.main.GetComponent<CameraFollow>().maxXAndY = cameraLimits.max;
		Camera.main.GetComponent<CameraFollow>().minXAndY = cameraLimits.min;
		Camera.main.GetComponent<CameraFollow>().levelZoom = cameraLimits.zoom;

		//currentWave = spawnWaves[0];
		StartCoroutine("WaveUpdate");
	}
	IEnumerator WaveUpdate()
	{
		yield return new WaitForSeconds(1);
		var evnt = spawnWaves[waveCount].spawnSeqs[seqCount].spawnEvents[eventCount];
		if(evnt.enemyType == EnemyType.SOLDIER){

		}
	}
}

[System.Serializable]
public class CameraLimits
{
	public Vector2 max;
	public Vector2 min;
	public int zoom;
}

[System.Serializable]
public class Wave
{
	public float delayNextSeq;
	public Sequence[] spawnSeqs;

}

[System.Serializable]
public class Sequence
{

	public Vector2 pos;
	public float delayNextEvent;
	public SpawnEvent[] spawnEvents;

}

[System.Serializable]
public class SpawnEvent
{
	public CombatZone.EnemyType enemyType;
	public Enemy.Equipped equipped;
	public Vector2 offset;

}
