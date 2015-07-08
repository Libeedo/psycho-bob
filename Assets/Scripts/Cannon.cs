using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {
	public GameObject fodder;
	public float power;
	public float delay;
	public float spawnRate;
	private Transform aim;
	private Transform shoot;
	private Animator anim;
	// Use this for initialization
	void Start () {
		aim = transform.Find ("aim").transform;
		shoot = transform.Find ("shoot").transform;
		InvokeRepeating("Queef",delay,spawnRate);
		anim = GetComponent<Animator>();
	}
	
	void Queef()
	{
		//print ("queef");
		GameObject fetus = (GameObject)Instantiate(fodder, shoot.position, transform.rotation);
		Vector3 vecc = (aim.position - transform.position).normalized * power;
		fetus.GetComponent<Rigidbody2D>().velocity = vecc;
		anim.Play ("CannonQueef");
		fetus.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-500f,500f));
	}
}
