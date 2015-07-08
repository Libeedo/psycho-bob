using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public float spawnTime = 5f;		// The amount of time between each spawn.
	public float spawnDelay = 3f;		// The amount of time before spawning starts.

	public int spawnMax = -1;  // negative == infinite spawn; change in inspector for finite 
	public bool respawnDead = false;   //replenish spawnCount when enemy dies

	protected int spawnCount = 0;
	



	public bool spawnAnim = false;
	public AudioClip[] spawnFX;
	protected bool canSpawn = true;
	void Start()
	{
		StartSpawn ();
	}
	public void StartSpawn ()
	{
		//spawnDelay += .001f;
		CancelInvoke();
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}
	public void StopSpawn()
	{
		CancelInvoke();
	}

	public virtual void Spawn ()
	{
		//print (spawnCount + "   " + spawnMax+"  "+Level.enemyCount + "   " + Level.instance.enemyMax);


		if(spawnMax >0){ //not inifiite (-1)
			if(spawnCount >= spawnMax || Level.enemyCount >= Level.instance.enemyMax){  //maximum enemies alive for spawner || level?
				//print ("waaa?");
				canSpawn = false;
			}else{
				spawnCount++; 
				canSpawn = true;
			}

		}else{//infinite
			spawnCount++; 
			canSpawn = true;
		}

			//print ("SPAWN "+canSpawn);



	}
	public void subtractSpawnCount(int sub)//subtract from spawnCount when enemy dies so more can spawn from this spawner if spawn max not infinite 
	{
		spawnCount -= sub;
	}

}
