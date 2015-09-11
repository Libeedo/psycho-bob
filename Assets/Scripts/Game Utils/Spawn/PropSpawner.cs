using UnityEngine;
using System.Collections;

public class PropSpawner : MonoBehaviour {

	public float spawnTime = 5f;		// The amount of time between each spawn.
	public float spawnDelay = 3f;		// The amount of time before spawning starts.
	

	
	
	public GameObject[] props;		// Array of enemy prefabs.
	public GameObject chuteRef;
	//private GameObject lastSpawn;
	
	//public bool paratrooperMode = false;// for respawn dead,
	
	//public float speed;
	private  JointAngleLimits2D limits;

	//GameObject go;
	void Start()
	{
		StartSpawn ();
	}
	public void StartSpawn ()
	{
		CancelInvoke();
		InvokeRepeating("Spawn", spawnDelay, spawnTime);
	}
	public void StopSpawn()
	{
		CancelInvoke();
	}
	
	void Spawn ()
	{

		// Instantiate a random enemy.
		int enemyIndex = Random.Range(0, props.Length);
		GameObject go = (GameObject)Instantiate(chuteRef, transform.position, transform.rotation);
		var p = go.transform.Find("parachute");
		p.GetComponent<Parachute>().attached = Parachute.Attached.PROP;
		p.GetComponent<Rigidbody2D>().gravityScale = -0.85f;

		GameObject rd = Instantiate(props[enemyIndex], transform.position, Quaternion.identity) as GameObject;
		HingeJoint2D jnt = go.transform.Find("harness").gameObject.AddComponent<HingeJoint2D>();
		jnt.connectedBody = rd.transform.GetComponent<Rigidbody2D>();
		jnt.useLimits = true;
		//limits = jnt.limits;
		limits.min = -30;
		limits.max = 30;
		jnt.limits = limits;


		//rd.transform.parent = go.transform;
		rd.transform.GetComponent<Explosive>().xMode = Explosive.XplodeMode.CHUTE;
		rd.transform.GetComponent<Explosive>().chuteCS = go.transform.Find("parachute").GetComponent<Parachute>();

		//Destroy (rd,12f);
		//Destroy (go,12f);
	}


}
