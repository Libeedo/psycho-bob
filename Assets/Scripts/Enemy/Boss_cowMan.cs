using UnityEngine;
using System.Collections;

public class Boss_cowMan : MonoBehaviour {
	private Animator anim;	
	public float HP = 2;

	public GameObject explRef;

	private bool headOpen = false;
	private float boost =6600f;
	private bool attacking = false;
	public ParticleSystem[] milk;
	private ParticleSystem faceBlood;

	private bool switchDir;

	private Transform rider;
	private bool blownUp = false;

	private GameObject hitZone;

	void Start () {
		faceBlood = transform.Find ("blood").GetComponent<ParticleSystem>();
		anim = GetComponent<Animator> ();
		anim.Play ("Boss cowboy idle");
		InvokeRepeating("Move", 3f, 3f);
		rider = transform.Find ("cowRider").transform;
		hitZone = transform.Find("hitZone").gameObject;
	}

	void Move()
	{

		//if (Random.value >= 0.3) {
			if (Random.value >= 0.5) {
				if(switchDir){
					switchDir=false;
					anim.Play("Boss cowboy forward");
					//GetComponent<Rigidbody2D>().AddForce(new Vector2 (boost * -1f,0));
				}else{
					switchDir=true;
					anim.Play("Boss cowboy backward");
					//GetComponent<Rigidbody2D>().AddForce(new Vector2 (boost * 1f,0));
				}
				shootMilk();
			}
			
		//}

		if (Random.value >= 0.5) {
			if(headOpen){
				if(attacking){
					return;
				}
				hitZone.SetActive(false);
				rider.GetComponent<Animator>().Play ("boss_cowrider_outHead");
				anim.Play ("Boss cowboy head close");
				headOpen = false;

			}else{
				hitZone.SetActive(true);
				rider.GetComponent<Animator>().Play ("boss_cowrider_earDive");
				anim.Play ("Boss cowboy head opening");
				headOpen = true;
			}
		}
		if(headOpen && !attacking){

			if (Random.value >= 0.2) {
				StartCoroutine(Attack());
			}
		}
	}
	public void Hurt(float damage,bool blowdUp)
	{

		if(headOpen){
			if(blowdUp){
				if(blownUp){
					return;
				}
				StartCoroutine(notBlownUp());
			}
			HP -= damage;
			Level.instance.makeHitNum((Vector2)hitZone.transform.position+Vector2.up*3,damage);
			faceBlood.Emit(80);
			transform.GetComponent<HitColour>().Hurt(0.3f);
			rider.GetComponent<Animator>().Play ("boss_cowrider_hit");
			GetComponent<AudioSource>().Play();
			if(HP <= 0){
				Death();
			}
		}
	}
	void Death()
	{
		Vector3 pos = transform.position;
		pos.z = -0.32f;
		transform.position = pos;

		transform.Find ("deadBody3D").gameObject.SetActive(true);
		transform.Find ("body3D").gameObject.SetActive(false);

		transform.Find ("bodyColliders").gameObject.SetActive(false);
		transform.Find ("bodyDeadColliders").gameObject.SetActive(true);
		transform.Find ("hitZone").gameObject.SetActive(false);
		transform.Find ("hitZone").GetComponent<CircleCollider2D>().isTrigger = true;
		rider.GetComponent<BoxCollider2D>().isTrigger = true;
		this.tag = "ground";
		foreach (ParticleSystem ps in milk){
			ps.Play();
		}
		CancelInvoke("Move");
		rider.GetComponent<Animator>().Play ("boss_cowrider_dead");
		InvokeRepeating("pushBackAfterDeath",0,1f);
		this.enabled = false;
		//Level.instance.Level_1_BossDeath();
	}
	public void shootMilk()
	{
		foreach (ParticleSystem ps in milk){
			ps.Play();
		}
		StartCoroutine(StopMilk());
	}
	IEnumerator StopMilk()
	{
		yield return new WaitForSeconds(1f);
		foreach (ParticleSystem ps in milk){
			ps.Stop();
		}
	}

	IEnumerator Attack()
	{
		attacking = true;

		transform.Find ("headGear").GetComponent<Animator> ().Play ("boss_headGearShoot");
		yield return new WaitForSeconds(.7f);
		rider.GetComponent<Animator>().Play ("boss_cowrider_attack");
		rider.GetComponent<AudioSource> ().Play ();

		yield return new WaitForSeconds(.5f);

		GameObject go = (GameObject)Instantiate(explRef, new Vector3(transform.position.x - 10f,transform.position.y +10f,0),Quaternion.identity);

		//go.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1500f,400f));
		attacking = false;
		yield return new WaitForSeconds (0.5f);
		transform.Find ("headGear").GetComponent<Animator> ().Play ("boss_headGearStopShoot");
	}
	void pushBackAfterDeath()
	{
		GetComponent<Rigidbody2D>().AddForce(new Vector2 (50f,0));
	}
	IEnumerator notBlownUp()
	{
		blownUp = true;
		yield return new WaitForSeconds(0.75f);
		
		blownUp = false;
	}
}
