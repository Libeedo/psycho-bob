using UnityEngine;
using System.Collections;

public class CargoSpawner : Spawner {

	public GameObject cargoRef;
	public Cargo.CargoMode cargo = Cargo.CargoMode.PIPE;
	public bool randomCargo = false;
	public override void Spawn ()
	{
		base.Spawn ();
		if(randomCargo){
			cargo = GameUtiils.GetRandomEnum<Cargo.CargoMode>();
		}
		GameObject go = (GameObject)Instantiate(cargoRef, transform.position, transform.rotation);
		go.transform.GetComponent<Cargo>().cargo = cargo;

		
	}
}
