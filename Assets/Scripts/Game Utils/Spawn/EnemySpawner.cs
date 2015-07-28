using UnityEngine;
using System.Collections;

public class EnemySpawner : Spawner {

	public GameObject[] enemies;		// Array of enemy prefabs.
	//private GameObject enemy;
	private GameObject lastSpawn;
	
	public bool paratrooperMode = false;  //is parachuting enemy?
	
	public bool vehicleMode = false;
	
	public Enemy_Soldier.Equipped equipped = Enemy_Soldier.Equipped.NIL;
	
	public bool randomEquipped = false;
	public bool randomSquadEquipped = false;

	public float speed;
	public bool randomSpeed;// for parachuting, ground speed should always be the same
	private bool flip = false;	
	private float openChuteDelay;
	public bool spawnSquad;
	public bool sniperWalkToggle = true; //does the sniper walk then stop and shoot repeatedly

	private AudioSource aud;

	void Awake()
	{
		if(spawnAnim){
			aud = GetComponent<AudioSource>();
	
		}
	}
	public override void Spawn()
	{

		base.Spawn ();
		if(canSpawn){
			if(spawnAnim){
				GetComponent<Animator>().Play ("Spawner");

				aud.clip = spawnFX[UnityEngine.Random.Range (0, spawnFX.Length)];
				aud.Play ();
				//print ("wtf");
				//AudioSource.PlayClipAtPoint(spawnFX[i], transform.position,0.1f);
			}
			if(vehicleMode){
				transform.parent.Find ("passenger").gameObject.SetActive(false);
				StartCoroutine(passengerBack());
			}
			if(randomEquipped){
				equipped = (Enemy.Equipped)Random.Range(0,4);//GameUtiils.GetRandomEnum<Enemy_Soldier.Equipped>();
			}
			if(randomSpeed){
				speed = Random.Range(0,6);
			}
			if(Random.Range(0,10)>5){
				flip = true;
			}else{
				flip = false;
			}
			openChuteDelay = Random.Range(.5f,1.6f);

			if(spawnSquad){
				SpawnSquad();
			}else{
				MakeEnemy(transform.position);
			}
		}
	}
	private void MakeEnemy(Vector2 pos)
	{
		//int enemyIndex = Random.Range(0, enemies.Length);
		GameObject go = (GameObject)Instantiate(enemies[0], pos, transform.rotation);
		
		Enemy_Soldier esCS = go.GetComponent<Enemy_Soldier>();
		
		
		if(flip){
			esCS.Flip();
		}
		
		esCS.paratrooperMode = paratrooperMode;
		esCS.openChuteDelay = openChuteDelay;
		esCS.equipped = equipped;
		esCS.moveSpeed = speed;

		esCS.snipeWalk = sniperWalkToggle;

		
		//if replenishing spawncount from the dead add that shit functionality to the enemy
		if(respawnDead){
			esCS.spawner = gameObject;
			esCS.replenishSpawner = true;
		}
	}
	private void SpawnSquad()
	{
		var pos = transform.position;
		var offset = 2;
		var rnd = false;
		if(randomEquipped && !randomSquadEquipped){
			rnd = true;
		}
		for(int r = 0; r <= 2; r++){
			for(int i = 0; i <= 2;i++){
				//pos.x += offset;
				if(rnd){
					equipped  = (Enemy.Equipped)Random.Range(0,4);//GameUtiils.GetRandomEnum<Enemy_Soldier.Equipped>();
				}
				MakeEnemy(pos);
				spawnCount++;
				pos.x += 2;
			}
			pos.x = transform.position.x +offset;
			if(offset>0){
				offset = 0;
			}else{
				offset = 2;
			}
			pos.y -= 6;
		}
		spawnCount--;//base class already adds 1 
	}
	IEnumerator passengerBack()
	{			
		
		// ... pause briefly
		yield return new WaitForSeconds(2);
		transform.parent.Find ("passenger").gameObject.SetActive(true);
	}
}
