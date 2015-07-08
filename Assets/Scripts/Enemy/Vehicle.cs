using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour {


	public GameObject explosion;			// Prefab of explosion effect.
	public GameObject deadHeli;

	public float HP = 15;
	public AudioClip[] soundFX;
	private bool dead = false;
	private bool blownUp = false;
	private ParticleSystem smoke;
	private ParticleSystem pilotBlood;

	public bool haulinCargo = false;
	private Cargo cargoCS;
	public Cargo.CargoMode cargoType;
	public GameObject ropeRef;
	//public bool strafeMode = false;
	private float pilotHP = 16;


	void Start()
	{
	
		smoke = transform.Find("smoke").GetComponent<ParticleSystem>();
		pilotBlood = transform.Find("hitZonePilot").transform.Find("blood").GetComponent<ParticleSystem>();
		if(haulinCargo){
			var rope = Instantiate(ropeRef,transform.position, Quaternion.identity) as GameObject;
			cargoCS = rope.GetComponent<Cargo>();

			cargoCS.cargo = cargoType;

			rope.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
		}//else{
			//heliT.GetComponent<SliderJoint2D>().connectedAnchor = transform.position;
		//}
	}

	public void Hurt(float damage,bool blowdUp)
	{
		//print("hutr");
		if(blowdUp){//explosive hurt?
			if(blownUp){//already just blown up?, wait
				return;
			}
			StartCoroutine(notBlownUp());
		}
		HP -= damage;

		Level.instance.makeHitNum((Vector2)transform.position,damage);

		smoke.maxParticles += (int)damage * 10;

		GetComponent<AudioSource>().clip = soundFX[Random.Range(0,soundFX.Length)];
		GetComponent<AudioSource>().Play ();
		if(HP <= 0 && !dead){
			// ... call the death function.
			Explode ();

		}
	
	}
	public void PilotHurt(float damage,bool blowdUp)
	{
		if(blowdUp){//explosive hurt?
			if(blownUp){//already just blown up?, wait
				return;
			}
			StartCoroutine(notBlownUp());
		}
		pilotHP -= damage;
		pilotBlood.Emit(80);
		if(pilotHP  <= 0 && !dead){
			// ... call the death function.
			PilotDeath();
			
		}
		
	}
	void PilotDeath()
	{
		transform.Find ("pilotDamage").gameObject.SetActive(true);
		GetComponent<Rigidbody2D>().isKinematic = false;
		GetComponent<Rigidbody2D>().gravityScale = 1f;
		GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.None;//fixedAngle = false;

		transform.Find("hitZone").GetComponent<Rigidbody2D>().isKinematic = false;
		transform.Find("hitZone").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;//fixedAngle = false;


		if(haulinCargo){

			cargoCS.Drop();
		}
		//heliT.GetComponent<GroundMove>().enabled = false;

		transform.Find("hitZone").gameObject.AddComponent<VehicleImpactExplosion>();
		transform.Find("hitZone").GetComponent<VehicleImpactExplosion>().vehicleCS = this;
		transform.Find("hitZonePilot").gameObject.SetActive(false);
		Destroy(GetComponent<MovePlatform>());
		//Destroy(transform.Find("girderSwing").transform.Find("spring").GetComponent<HingeJoint2D>());
		//rigidbody2D.AddTorque(5000f);//wtf?

	}
	void Explode()
	{

		dead = true;
		Level.instance.score.UpdateScore(1000,new Vector3(transform.position.x,transform.position.y+8f));
		Instantiate(explosion,transform.position, Quaternion.identity);//new Vector3(0, 0.01f, 0)));
		GameObject go = (GameObject)Instantiate(deadHeli,transform.position, Quaternion.Euler(new Vector3(0, 84f, 0)));
		Destroy (go,2f);
		//print (haulinCargo);
		if(haulinCargo){
			cargoCS.Death();
		}
		Destroy (gameObject);
		
		return;
		
	}
	IEnumerator notBlownUp()
	{
		blownUp = true;
		yield return new WaitForSeconds(0.75f);
		
		blownUp = false;
	}
	public void cockPitBroken()
	{
		transform.Find("hitZonePilot").gameObject.SetActive(true);
	}
	public Vector2 getSpeed()
	{
		return GetComponent<Rigidbody2D>().velocity;
	}
}
