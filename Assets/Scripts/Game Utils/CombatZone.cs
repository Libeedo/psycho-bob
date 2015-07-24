using UnityEngine;
using UnityEditor;
using System.Collections;

public class CombatZone : MonoBehaviour {

	public GameObject[] enemies;

	public CameraLimits cameraLimits;
	public float delayNextWave;

	private float delaySeq;
	private float delayEvnt;

	public Wave[] spawnWaves;

	//private Sequence currentSeq;
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

		//StartSeq ();

		StartCoroutine("StartWave");
	}

	IEnumerator StartWave()
	{
		var seqs = spawnWaves[waveCount].spawnSeqs; //get sequences
		delaySeq = spawnWaves [waveCount].delayNextSeq; //delay next sequence
		
		foreach (Sequence sq in seqs) { //for each sequence in wave
			delayEvnt = sq.delayNextEvent;
			foreach (SpawnEvent evnt in sq.spawnEvents) { //for each event in sequence
				//currentSeq = sq;
				//var evnt = sq.spawnEvents[eventCount]; //get spawn event
				int which = (int)evnt.enemyType; //get enemyType of event;
				var enemy = enemies [which];
				var pos = sq.pos + evnt.offset; // spawn position = sequence position + event offset;
				
				GameObject go = (GameObject)Instantiate(enemy, pos, Quaternion.identity);
				if(evnt.enemyType == EnemyType.SOLDIER){
					var esc = go.GetComponent<Enemy_Soldier>();
					//esc.
				}
				print (go);
				yield return new WaitForSeconds(delayEvnt);
				
			}

		
			yield return new WaitForSeconds(delaySeq);

		}

	}

	//IEnumerator WaveUpdate()
	//{
		//yield return new WaitForSeconds(1);

	//}
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
