using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Damage :MonoBehaviour{
	public Enemy enemyCS;
	public Transform t; //pointer to damage object on enemys chest

	private GameObject headDamagesAll; // head damages parent
	//private GameObject[] headDamages = new GameObject[5];//all head damage sprites
	private List<GameObject> headDamages = new List<GameObject>(); 

	//private int headDamage = 0;//how damaged the head is
	private ParticleSystem faceBlood;

	//bullet hole stuff
	//public GameObject bulletHolesAll;
	//private GameObject[] bulletHoles = new GameObject[16];
	private List<GameObject> bulletHoles = new List<GameObject>();
	//private int bulletHoleCount = 0;
	private ParticleSystem bodyBlood;


	//fire 
	//private float flameDamage = 1f;
	[HideInInspector]
	public bool onFire = false;
	private int maxFireCount = 8;
	private int fireCount = -1;

	private ParticleSystem flames;
	private ParticleSystem smoke;
	private GameObject flameLight;
	public GameObject flameHitRef;


	public bool blownUp = false; //just blown up? dont blow up again?
	private bool firedUp = false;
	[HideInInspector]
	public bool scoreDeath = false; //add points on death?
	public AudioClip[] bulletHitClips;

	private bool autoDestructMode = false;

	private AudioSource aud;

	public void AwakeAfter (Transform tt) {
		t = tt;
		//int count = 0;
		//HEAD DAMAGE 
		if(t.root.Find ("head Transform")){
			headDamagesAll = t.root.Find ("head Transform").Find ("head").Find ("headDamage").gameObject;

			foreach (Transform child in headDamagesAll.transform) {
				
				headDamages.Add (child.gameObject);  //add each bullet hole to list
				//unusedHeadDamages .Add (count);
				
				//count++;
			}
			//faceBlood = t.root.transform.Find ("head Transform").Find ("head").Find ("faceBlood").GetComponent<ParticleSystem>();
		}
		//bulletHolesAll = bHoles;
		//count = 0;
		foreach (Transform child in t.Find ("bulletHoles")) {//add each bullet hole to list
			//print (unusedBulletHoles.Count);
			bulletHoles.Add (child.gameObject);
			//unusedBulletHoles.Add (count);
			//count++;

		}
		//bodyBlood = t.Find ("bodyBlood").GetComponent<ParticleSystem>();

		flames = t.Find ("flame").GetComponent<ParticleSystem> ();
		smoke = t.Find ("smoke").GetComponent<ParticleSystem> ();
		flameLight = t.Find ("flameLight").gameObject;

		aud = t.gameObject.AddComponent<AudioSource>();
		aud.playOnAwake = false;
		aud.spatialBlend = 1f;
	}
	public void damageHead(float dmg)
	{
		//print ("headshot "+headDamage);
		//if(dead)return;
		if(headDamages.Count > 0){
			int rnd = Random.Range (0, headDamages.Count-1);
			//print ("hole count "+bulletHoleCount+"   "+bulletHoles.Length);
			var h = headDamages[rnd];
			h.SetActive(true);
			faceBlood = h.transform.Find ("blood").GetComponent<ParticleSystem>();
			faceBlood.transform.parent = null;
			faceBlood.Emit(120);
			Destroy(faceBlood.gameObject,2f);
			headDamages.Remove(h);

		}
		

		aud.clip = bulletHitClips[Random.Range (0, bulletHitClips.Length)];
		aud.Play();
	}
	public void destroyHead()
	{
		headDamages.Clear();
		t.Find ("neckBlood").GetComponent<ParticleSystem> ().Play ();
	}
	public void makeBodyBulletHole()
	{

		if(bulletHoles.Count >0){
			int rnd = Random.Range (0, bulletHoles.Count-1);
			//print ("hole count "+bulletHoleCount+"   "+bulletHoles.Length);
			var b = bulletHoles[rnd];
			b.SetActive(true);
			bodyBlood = b.transform.Find ("blood").GetComponent<ParticleSystem>();
			bodyBlood.transform.parent = null;
			bodyBlood.Emit(160);
			Destroy(bodyBlood.gameObject,2f);
			bulletHoles.Remove(b);
		}
		//bulletHoleCount++;

		aud.clip = bulletHitClips[Random.Range (0, bulletHitClips.Length)];
		aud.Play();
	
	}
	public void makeBodyBlood()//use a bunch of blood from the bulletholes for hard splat impacts from damage collider
	{
		var count = 0;
		for(int i = 0; i<bulletHoles.Count;i++){
			int rnd = Random.Range (0, bulletHoles.Count-1);
			//print ("hole count "+bulletHoleCount+"   "+bulletHoles.Length);
			var b = bulletHoles[rnd];
			//b.SetActive(true);
			bodyBlood = b.transform.Find ("blood").GetComponent<ParticleSystem>();
			bodyBlood.transform.parent = null;
			bodyBlood.Emit(160);
			Destroy(bodyBlood.gameObject,2f);
			count++;
			bulletHoles.Remove(b);
			if(count>3){
				return;
			}
		}

		
	}
	public void BlownUp()
	{
		//print (enemyCS.gameObject.name);
		//damageHead(1f);
		StartCoroutine (notBlownUp());
	}
	IEnumerator notBlownUp()
	{
		//print (blownUp);
		blownUp = true;
		yield return new WaitForSeconds(0.75f);
		scoreDeath = true;
		blownUp = false;
		//print ("not blown up");
	}
	public  void Flamed(){
		//print ("flamed " + onFire + "  " + firedUp);
		fireCount = maxFireCount;
		if (firedUp) { //if just recieved flame damage
			return;
		}else if (onFire) { //if still on fire, dont start a new one, just damage
			StartCoroutine("notFiredUp");
			enemyCS.FlameDamage (0.4f);
			return;
		}
		flames.Play ();
		onFire = true;
		StartCoroutine("notFiredUp");// so he doesnt get hurt by the flames he produces

		InvokeRepeating("FlameDamage",0f,0.5f);
		flameLight.SetActive(true);
		//enemyCS.FlameDamage (1f);
	
	}
	void FlameDamage()
	{
		//enemyCS.FlameDamage (0.5f);flameHit created below now does its own damage
		fireCount--;

		//GameObject e = (GameObject)
		Instantiate(flameHitRef,t.position, Quaternion.identity);//make flame hit to start other shit on fire


		if(fireCount == 0){
			fireCount = -1;
			CancelInvoke();
			onFire = false;
			flames.Stop();
			smoke.Play();
			StartCoroutine("StopSmoke");
			enemyCS.StopFlame();
			flameLight.SetActive(false);
			//stopFire();-->stop smoke
			
		}
		//print ("flame damage "+fireCount);
	}
	IEnumerator notFiredUp()
	{
		//print (blownUp);
		firedUp = true;
		yield return new WaitForSeconds(0.4f);

		firedUp = false;
		//print ("not fired up");
	}
	IEnumerator StopSmoke()
	{
		yield return new WaitForSeconds (4f);
		//flameLight.SetActive(false);
		smoke.Stop ();
		if(autoDestructMode){
			Destroy (gameObject);
		}
	}

	public void unHittable()//if on fire when  you can no long hit the enemys dead body, wait and put out the fire/smoke before destroying this gameobject
	{
		if(onFire){
			autoDestructMode = true;
		}else{
			smoke.Stop ();
			Destroy (gameObject);
		}
	}



}

