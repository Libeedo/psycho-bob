using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CombatZone : MonoBehaviour {

	public GameObject[] enemies;

	private CameraLimits cameraLimits = new CameraLimits();

	public int cameraZoneWidth;
	public int cameraZoneHeight;
	public int cameraZoom;

	public GameObject zoneSpawnPoint;
	private CameraLimits levelCameraLimits = new CameraLimits();
	[HideInInspector]
	public float delayNextWave;

	private float delaySeq;
	private float delayEvnt;
	[HideInInspector]
	public List<Wave> spawnWaves = new List<Wave>();
	[HideInInspector]
	public List<GameObject> waveEnemies = new List<GameObject> (); //List of current wave of enemies;
	//private Sequence currentSeq;
	//private SpawnWave currentWave;
	//public SpawnSequence spawnSequence;
	//public SpawnEvent spawnEvent;

	int waveCount = 0;
	//int seqCount = 0;
	//int eventCount = 0;
	private Animation door2Anim;
	public enum EnemyType{
		SOLDIER,
		DICKBUG,
		DICKBAT
	}

	private Camera cam;
	
	void Awake()
	{
		cam = Camera.main;

	}
	public void StartZone()
	{
		//get camerazone basd on offset from origin
		cameraLimits.min.x = transform.position.x;
		cameraLimits.min.y = transform.position.y;
		cameraLimits.max.x = transform.position.x + cameraZoneWidth;
		cameraLimits.max.y = transform.position.y + cameraZoneHeight;
		cameraLimits.zoom = cameraZoom;
		print (cameraLimits.max.x+"  "+cameraLimits.max.y);


		var camm = cam.GetComponent<CameraFollow>();
		//print("startZone "+camm.maxXAndY);
		levelCameraLimits.max = camm.maxXAndY;
		levelCameraLimits.min = camm.minXAndY;
		levelCameraLimits.zoom = camm.levelZoom;
		camm.maxXAndY = cameraLimits.max;
		camm.minXAndY = cameraLimits.min;
		camm.levelZoom = cameraLimits.zoom;

		zoneSpawnPoint.GetComponent<SpawnPoint>().EnableSpawn();

		Transform door2;
		if (door2 = transform.Find ("door2")) {
			door2Anim = door2.GetComponent<Animation>(); 



		}
		Transform door1;
		if (door1 = transform.Find ("door1")) {
			door1.GetComponent<Animation>().Play ();
		}
		//StartSeq ();

		StartCoroutine("PreStartWave");
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
		var camm = cam.GetComponent<CameraFollow>();
		camm.maxXAndY = levelCameraLimits.max;
		camm.minXAndY = levelCameraLimits.min;
		camm.levelZoom = levelCameraLimits.zoom;
		foreach (AnimationState state in door2Anim) {
			state.speed = -1;
		}
		door2Anim.Rewind();
		door2Anim.Play();
		Destroy (this);
	}
	IEnumerator PreStartWave()
	{
		cam.enabled = false;
		transform.Find("camera").gameObject.GetComponent<Camera>().enabled = true;
		transform.Find("camera").GetComponent<Animation>().Play();
		Level.instance.PausePlayer();
		
		yield return new WaitForSeconds (1f);
		door2Anim.Play();
		yield return new WaitForSeconds (2f);
		cam.transform.position = transform.Find("camera").position;
		cam.enabled = true;
		transform.Find("camera").gameObject.GetComponent<Camera>().enabled = false;
		Level.instance.unPausePlayer();
		StartCoroutine("StartWave");
	}
	IEnumerator StartWave()
	{
		//print ("WAVE " + waveCount);


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
				var pos = evnt.offset; // spawn position = sequence position + event offset;
				//print (evnt.offset);
				GameObject go = (GameObject)Instantiate(enemy, pos, Quaternion.identity);
				Level.instance.makeTeleFlash(pos);
				waveEnemies.Add(go);
				var r = go.AddComponent<RemoveFromSpawnWave>();
				r.combatZone = this;

				if(evnt.enemyType == EnemyType.SOLDIER){

					var esc = go.GetComponent<Enemy_Soldier>();
					if(evnt.equipped == Enemy.Equipped.RANDOM){
						var rnd = Random.Range(0,4);
						//print (rnd);
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
					if(Random.Range(0,10)>5){
						esc.Flip();
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
				//print (go);
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
