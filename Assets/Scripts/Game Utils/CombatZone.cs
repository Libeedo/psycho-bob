using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CombatZone : MonoBehaviour {

	public GameObject[] enemies;

	public CameraLimits cameraLimits;
	private CameraLimits levelCameraLimits = new CameraLimits();

	public float delayNextWave;

	private float delaySeq;
	private float delayEvnt;

	public List<Wave> spawnWaves = new List<Wave>();
	public List<GameObject> waveEnemies = new List<GameObject> (); //List of current wave of enemies;
	//private Sequence currentSeq;
	//private SpawnWave currentWave;
	//public SpawnSequence spawnSequence;
	//public SpawnEvent spawnEvent;

	int waveCount = 0;
	//int seqCount = 0;
	//int eventCount = 0;

	public enum EnemyType{
		SOLDIER,
		DICKBUG,
		DICKBAT
	}


	
	// Update is called once per frame
	//void Update () {
	
	//}
	public void StartZone()
	{

		var cam = Camera.main.GetComponent<CameraFollow>();
		print("startZone "+cam.maxXAndY);
		levelCameraLimits.max = cam.maxXAndY;
		levelCameraLimits.min = cam.minXAndY;
		levelCameraLimits.zoom = cam.levelZoom;
		cam.maxXAndY = cameraLimits.max;
		cam.minXAndY = cameraLimits.min;
		cam.levelZoom = cameraLimits.zoom;

		//StartSeq ();

		StartCoroutine("StartWave");
	}
	public void NextWave()
	{
		if(waveCount >= spawnWaves.Count){
			EndZone();
		}else{
			StartCoroutine("StartWave");
		}
	}
	void EndZone()
	{
		Camera.main.GetComponent<CameraFollow>().maxXAndY = levelCameraLimits.max;
		Camera.main.GetComponent<CameraFollow>().minXAndY = levelCameraLimits.min;
		Camera.main.GetComponent<CameraFollow>().levelZoom = levelCameraLimits.zoom;
		Destroy (gameObject);
	}
	IEnumerator StartWave()
	{
		print ("WAVE " + waveCount);
		yield return new WaitForSeconds (delayNextWave);

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
				waveEnemies.Add(go);
				var r = go.AddComponent<RemoveFromSpawnWave>();
				r.combatZone = this;

				if(evnt.enemyType == EnemyType.SOLDIER){

					var esc = go.GetComponent<Enemy_Soldier>();
					if(evnt.equipped == Enemy.Equipped.RANDOM){
						var rnd = Random.Range(0,4);
						print (rnd);
						esc.equipped = (Enemy.Equipped)rnd;//GameUtiils.GetRandomEnum<Enemy.Equipped>();
					}else{
						esc.equipped = evnt.equipped;
					}
					if(evnt.paratrooper){
						esc.moveSpeed = Random.Range(0,6);
						esc.paratrooperMode = true;
					}else{
						esc.moveSpeed = evnt.speed;
					}


				}else if(evnt.enemyType == EnemyType.DICKBUG){

					var esc = go.GetComponent<Enemy>();
					if(evnt.equipped == Enemy.Equipped.RANDOM){

						var rnd = Random.Range(0,4);
						print (rnd);
						esc.equipped = (Enemy.Equipped)rnd;
					}else{
						esc.equipped = evnt.equipped;
					}

				}
				print (go);
				yield return new WaitForSeconds(delayEvnt);
				
			}

		
			yield return new WaitForSeconds(delaySeq);
			 
		}
		waveCount++;
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
	public float zoom;
}

[System.Serializable]
public class Wave
{
	public float delayNextSeq;
	public List<Sequence> spawnSeqs = new List<Sequence>();

}

[System.Serializable]
public class Sequence
{

	public Vector2 pos;
	public float delayNextEvent;
	public List<SpawnEvent> spawnEvents = new List<SpawnEvent>();

}

[System.Serializable]
public class SpawnEvent
{
	public CombatZone.EnemyType enemyType;
	public Enemy.Equipped equipped;
	//public bool randomEquipped;
	public bool paratrooper;
	public float speed;
	//public bool randomSpeed;
	public Vector2 offset;
	
}
