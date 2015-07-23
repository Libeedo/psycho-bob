using UnityEngine;
using System.Collections;

public class VehicleSpawner : Spawner {

	public bool spawnEnemyFromVehicle;
	public float spawnEnemyRate;

	public bool haulinCargo = false;
	public Cargo.CargoMode cargo = Cargo.CargoMode.GIRDER;
	public bool randomCargo = false;

	public GameObject vehicle;


	public override void Spawn ()
	{

		base.Spawn ();
		GameObject go = (GameObject)Instantiate(vehicle, transform.position, transform.rotation);

		if(spawnEnemyFromVehicle){
			go.transform.Find("helicopter").transform.Find("spawner").GetComponent<Spawner>().spawnTime = spawnEnemyRate;
		}else{
			go.transform.Find("helicopter").transform.Find("spawner").gameObject.SetActive(false);
			go.transform.Find("helicopter").transform.Find("passenger").gameObject.SetActive(false);
		}
		if(haulinCargo){
			go.GetComponent<Vehicle>().haulinCargo = true;
			if(randomCargo){
				cargo = GameUtiils.GetRandomEnum<Cargo.CargoMode>();
			}
			go.GetComponent<Vehicle>().cargoType = cargo;
		}
			
		
	}

}
