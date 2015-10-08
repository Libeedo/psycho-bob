using UnityEngine;
using System.Collections;

public class VehicleSpawner : Spawner {

	public bool spawnEnemyFromVehicle;
	public float spawnEnemyRate;

	public bool haulinCargo = false;
	public Cargo.CargoMode cargo = Cargo.CargoMode.GIRDER;
	public bool randomCargo = false;

	public GameObject vehicle;

	public bool facingRight = true;
	public Vector2 max;
	public Vector2 min;

	public bool moveX;
	public bool moveY;
	public int speed;

	public float destroyDelay;

	public override void Spawn ()
	{

		base.Spawn ();
		if(canSpawn){
			GameObject go = (GameObject)Instantiate(vehicle, transform.position, transform.rotation);
			//Level.instance.makeTeleFlash(transform.position);
			if(spawnEnemyFromVehicle){
				go.transform.Find("spawner").GetComponent<Spawner>().spawnTime = spawnEnemyRate;
			}else{
				go.transform.Find("spawner").gameObject.SetActive(false);
				go.transform.Find("passenger").gameObject.SetActive(false);
			}
			var mp = go.GetComponent<MovePlatform>();
			mp.maxX = max.x;
			mp.minX = min.x;
			mp.maxY = max.y;
			mp.minY = min.y;
			mp.moveX = moveX;
			mp.moveY = moveY;
			mp.maxSpeed = speed;
			if(!facingRight){
				var s = transform.localScale;
				s.x = -1;
				go.transform.localScale = s;
			}
			if(destroyDelay > 0)
			{
				Destroy (go,destroyDelay);
			}
			if(haulinCargo){
				go.GetComponent<Vehicle>().haulinCargo = true;
				if(randomCargo){
					cargo = GameUtiils.GetRandomEnum<Cargo.CargoMode>();
				}
				go.GetComponent<Vehicle>().cargoType = cargo;
			}
			if(respawnDead){
				var rfs = go.AddComponent<RemoveFromSpawner>();
				rfs.spawner = this;
			}
		}
		
	}

}
