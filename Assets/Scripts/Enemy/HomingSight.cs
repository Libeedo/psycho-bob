using UnityEngine;
using System.Collections;

public class HomingSight : MonoBehaviour {
	private Animator anim;
	private float crankSpeed = 1f;
	private float rot = 0;
	private bool crankDir;
	private bool cranking = true;
	private bool shooting = false;
	public GameObject bulletRef;
	private Transform bulletPos;
	public AudioClip[] shootFX;
	void Start () {
		rot = transform.rotation.z;
		anim = transform.root.transform.GetComponent<Animator>();
		bulletPos = transform.Find ("shotPos");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(shooting){
			return;

		}
		Vector3 lookPos = Level.instance.GetPlayerTransform().position - transform.position;
		float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
		var r = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation,r,crankSpeed * Time.deltaTime);


		var z = transform.eulerAngles.z;
		var diff = z - rot;

		if(Mathf.Abs(diff)> 0.1f){
			cranking = true;
			//print (diff+"  "+crankDir);
			if(z>rot){
				if(!crankDir){

					crankDir = true;
					//print ("bigger"+diff);
					anim.Play("assTank_crankCW");
				}
			}else{
				if(crankDir){
				
					crankDir = false;
					//print ("smaller"+diff);
					anim.Play("assTank_crankCCW");
				}
			}
		}else if(cranking){
			shooting = true;
			cranking = false;
			anim.Play ("noCrank");
			anim.SetTrigger("shoot");
			StartCoroutine("Shoot");
		}
		rot = transform.eulerAngles.z;
	}
	IEnumerator Shoot()
	{
		var plPos = Level.instance.GetPlayerTransform().position;
		plPos.y += 2;
		var moveToPos = ( plPos-transform.position).normalized * 50000f;
		AudioSource.PlayClipAtPoint(shootFX[shootFX.Length-1], transform.position);

		yield return new WaitForSeconds(1.3f);
		AudioSource.PlayClipAtPoint(shootFX[Random.Range (0, shootFX.Length-1)], transform.position);
		var g = (GameObject)Instantiate(bulletRef, bulletPos.position, Quaternion.identity);

		g.GetComponent<Enemy_Bullet>().moveToPos = moveToPos;

		yield return new WaitForSeconds(0.2f);
		shooting = false;
		cranking = true;
		if(crankDir){
			anim.Play("assTank_crankCCW");
		}else{
			anim.Play("assTank_crankCW");
		}
	}
}
