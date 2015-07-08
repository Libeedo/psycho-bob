using UnityEngine;
using System.Collections;

public class Cargo : MonoBehaviour {

	public enum CargoMode
	{
		PIPE,
		BOUNCE,
		GIRDER,
		BEAM_ANGLED,
		PLATFORM,
		WALL_SHORT,
		WALL_ANGLE
		
	}
	public GameObject[] cargoRefs;
	public CargoMode cargo = CargoMode.PIPE;
	public bool vehicleCargo = false;//on a vehicle? or just a rope//future drone im gonna animate
	//private GroundMove groundMove;
	private GameObject t;

	//public bool groundMoveMode = true;

	void Start()
	{



		switch(cargo){
		case CargoMode.PIPE:
			t =  Instantiate(cargoRefs[0],transform.position, Quaternion.identity) as GameObject;

			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.BOUNCE:
			t =  Instantiate(cargoRefs[1],transform.position, Quaternion.identity) as GameObject;

			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.GIRDER:
			t =  Instantiate(cargoRefs[2],transform.position, Quaternion.identity) as GameObject;

			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.BEAM_ANGLED:
			t =  Instantiate(cargoRefs[3],transform.position, Quaternion.identity) as GameObject;
		
			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.PLATFORM:
			t =  Instantiate(cargoRefs[4],transform.position, Quaternion.identity) as GameObject;
		
			t.transform.parent = transform.root.transform;
			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.WALL_SHORT:
            t =  Instantiate(cargoRefs[5],transform.position, Quaternion.identity) as GameObject;
	
			t.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		case CargoMode.WALL_ANGLE:
            t =  Instantiate(cargoRefs[6],transform.position, Quaternion.identity) as GameObject;
	
			t.transform.Find ("wall").GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			break;
		}
		//GameObject c =  Instantiate(t,transform.position, Quaternion.identity) as GameObject;
		//c.GetComponent<HingeJoint2D>().connectedBody = transform.rigidbody2D;
		//if (justRopeMode) {
			//c.GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
		//}

		//if (cargo == CargoMode.PIPE) {
			///c.GetComponent<HingeJoint2D>().connectedBody = rigidbody2D;

		//}else{
			//c.transform.parent = transform.root.transform;

		//}
	}
	public void Drop()
	{
		Destroy (GetComponent<HingeJoint2D>());
	}
	public void Death()
	{
		print ("daeth");
		Destroy (t);
		Destroy (gameObject);
	}
}
