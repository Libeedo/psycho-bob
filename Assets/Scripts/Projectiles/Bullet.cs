using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public int damage = 1;
	public GameObject explosion;
	private int groundLayerMsk;
	private RaycastHit2D hit2;
	void Start () 
	{
		/*int layerMsk1 = 1 << LayerMask.NameToLayer("Enemies");
		int layerMsk3 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");*/
		int layerMsk4 = 1 << LayerMask.NameToLayer("Equipped");
		groundLayerMsk = layerMsk4;//layerMsk1 | layerMsk2 | layerMsk3 | layerMsk4;
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 4f);
	}


	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		 //Instantiate the explosion where the rocket is with the random rotation.
			Instantiate(explosion, transform.position, randomRotation);
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print (col.tag);
		// If it hits an enemy...
		if (col.tag == "Enemy") {
						// ... find the Enemy script and call the Hurt function.
				col.transform.root.GetComponent<Enemy> ().Shot (false, damage, GetComponent<Rigidbody2D> ().velocity * 2, transform.position);//new Vector2 (rigidbody2D.velocity.x * 80f, rigidbody2D.velocity.y*30f)

				// Call the explosion instantiation.
				OnExplode ();

				// Destroy the rocket.
				Destroy (gameObject);
		} else if (col.tag == "EnemyHead") {
				// ... find the Enemy script and call the Hurt function.
				col.transform.root.GetComponent<Enemy> ().Shot (true, damage + 1, GetComponent<Rigidbody2D> ().velocity * 2.5f, transform.position);//new Vector2 (rigidbody2D.velocity.x * 10f, rigidbody2D.velocity.y)
	
				// Call the explosion instantiation.
				OnExplode ();
	
				// Destroy the rocket.
				Destroy (gameObject);
				/*} else if (col.tag == "EnemyFetus") {
		print ("hit");
		// ... find the Enemy script and call the Hurt function.
		col.transform.GetComponent<Enemy_Fetus> ().Hurt (damage);
		//Destroy (col.gameObject);
		// Call the explosion instantiation.
		OnExplode ();

		// Destroy the rocket.
		Destroy (gameObject);

}*/

		}else if (col.tag == "Parachute") {
				//print ("chute hit "+col.gameObject.GetComponent<DistanceJoint2D>().connectedBody.name);

				//Transform e = col.gameObject.GetComponent<Parachute>().enemy.transform;
				col.GetComponent<Parachute> ().Hurt (false);
		
		//col.gameObject.GetComponent<Parachute>().enemy.transform.GetComponent<Enemy>().chuteHurt(false);



				OnExplode ();
	
				// Destroy the rocket.
				//Destroy (gameObject);
		}
		// Otherwise if it hits a bomb crate...
		else if (col.tag == "Explosive") {
				// ... find the Bomb script and call the Explode function.
				col.GetComponent<Explosive> ().Hurt (damage, GetComponent<Rigidbody2D>().velocity, transform.position);


				// Destroy the rocket.
				Destroy (gameObject);
		} else if (col.tag == "Deflective") {
				//RaycastHit2D hit = Physics2D.Linecast (transform.position, col.transform.position, groundLayerMsk);
				//Debug.DrawRay(hit.point, hit.normal * 10, Color.red);
				//Debug.DrawLine (hit.point, hit.normal * 5, Color.red);

				col.GetComponent<Deflective> ().Deflect (new Vector2 (GetComponent<Rigidbody2D>().velocity.x * 5f, GetComponent<Rigidbody2D>().velocity.y) * (15f));
				Deflect ();


				OnExplode ();
		} else if (col.tag == "HitZone") {
				col.GetComponent<HitZone>().hitDel(damage,false);
				/*return;
				HitZone.ObectType ot = col.GetComponent<HitZone> ().objectType;
				if (ot == HitZone.ObectType.COWMAN) {
					
					col.transform.parent.GetComponent<Boss_cowMan> ().Hurt (damage, false);
				} else if (ot == HitZone.ObectType.HELICOPTER) {
					if (col.GetComponent<HitZone> ().pilotMode) {
							col.transform.parent.GetComponent<Vehicle> ().PilotHurt (5);
					} else {
							col.transform.parent.GetComponent<Vehicle> ().Hurt (5, true);
					}
				}*/
				OnExplode ();
				Destroy (gameObject);
		} else if (col.tag == "Debris") {
				OnExplode ();
				col.GetComponent<DebrisPiece> ().Hurt (damage);
				Destroy (gameObject);
		/*} else if (col.tag == "Nurse") {
		print ("NURSE HIT CODE NEEDED");*/
		}else if (col.tag == "Switch"){
		
			col.GetComponent<Deflective> ().Deflect (new Vector2 (GetComponent<Rigidbody2D>().velocity.x * 5f, GetComponent<Rigidbody2D>().velocity.y) * (15f));
			Deflect ();
			OnExplode();
			col.transform.GetComponent<LeverTrigger>().TriggerLever();
			//Destroy (gameObject);
		}else if(col.tag != "Player" ){
			//Debug.Log ("hit player "+ col.gameObject.name);
			// Instantiate the explosion and destroy the rocket.
			OnExplode();
			Destroy (gameObject);
		}

		//if(LayerMask.LayerToName(col.gameObject.layer != "Debris"
		//Debug.Log (LayerMask.LayerToName(col.gameObject.layer));

	}
	private void Deflect()
	{
		
		//print (transform.right+"    "+transform.position+(-transform.right));
		if (hit2 = Physics2D.Raycast(transform.position + (-transform.right), transform.right,2,groundLayerMsk)){
			//print (hit2.normal+"  "+hit2.normal.normalized+"  "+hit2.point);
			Vector2 vecc2 = Vector2.Reflect (GetComponent<Rigidbody2D>().velocity, hit2.normal.normalized);
			GetComponent<Rigidbody2D>().velocity = vecc2;
			
			var v = GetComponent<Rigidbody2D>().velocity;
			float angle = Mathf.Atan2 (v.y, v.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
	}
}
