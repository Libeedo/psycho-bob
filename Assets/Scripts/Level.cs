using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;


public class Level : MonoBehaviour {
	public static Level instance;
	public int sortingNumber = 0;

	public static int enemyCount = 0;
	public int enemyMax; //number of enemies alive at once


	//public int bodyCount = 0;///limits amount of bodies deletes the last for every new dead body
	private int maxBodyCount = 30;
	public List<Enemy> enemies = new List<Enemy>();// all enemy CS
	private List<GameObject> bodies = new List<GameObject>(); //dead bodies
	//public GameObject bodyRef;////-------------------------------------------

	public SpawnPoint spawnPoint;
	private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

	List<HitNumClass> hitNums = new List<HitNumClass>(); 
	private int hitNumCount = 0;
	public GameObject hitNumRef;
	private Color32 playerHitNumClr;
	private Color32 enemyHitNumClr;

	public GameObject heroDead;
	private int lives = 3;
	private Text livesTxt;

	public GameObject nurse;
	private Transform playerT;		// Reference to the player's transform.
	private PlayerControl playerCS;
	[HideInInspector]
	public Score score;

	private GameObject nursePointer;
	private GameObject nursePointerHead;

	private GameObject deathPanel;
	private GameObject winPanel;
	private GameObject optionsPanel;

	//private bool musicOn = true;

	private CameraFollow camCS;

	public Pauser pauser;

	public GameObject teleFlashRef;
	public GameObject hitFXref;

	void Awake ()
	{
		instance = this;

		camCS = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

		playerT = GameObject.FindGameObjectWithTag("Player").transform;
		playerCS = playerT.GetComponent<PlayerControl>();
		nurse = GameObject.Find ("Nurse").gameObject;
		nursePointer = GameObject.Find ("nurse_UIpointer").gameObject;
		nursePointerHead = GameObject.Find ("nurse_PointerHead").gameObject;
		deathPanel = GameObject.Find ("DeathPanel").gameObject;
		winPanel = GameObject.Find ("WinPanel").gameObject;
		optionsPanel = GameObject.Find ("OptionsPanel").gameObject;

		pauser = transform.Find ("Pauser").GetComponent<Pauser>();
		//nursePointer.transform.parent = null;
		//nursePointer.GetComponent<OffscreenIndicator>().goToTrack = nurse;
		nursePointer.SetActive(false);
		nursePointerHead.SetActive(false);
		deathPanel.SetActive(false);
		winPanel.SetActive(false);
		optionsPanel.SetActive(false);
	}
	void Start () {
		//maxBodyCount = enemyMax/3;// + (enemyMax/10);
		score = GameObject.Find("Score").GetComponent<Score>();
		livesTxt = GameObject.Find ("livesTXT").GetComponent<Text>();
		livesTxt.text = lives.ToString();

		findSpawnPoints();
		makeHitNums();
		playerHitNumClr = new Color32 (51, 255, 0, 255);
		enemyHitNumClr = new Color32 (255, 0, 0, 255);
	
		Vector3 spawnSpot = spawnPoint.gameObject.transform.Find("spawnLoc").transform.position;//if error,  one spawnp0int needs to have selected checked in the inspector
		spawnSpot.z = 0f;

		playerT.transform.position = spawnSpot;
		playerT.gameObject.SetActive(false);
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = true;
		StartCoroutine(StartLevel());
	}
	public void ToggleNursePointer(bool which)
	{
		nursePointer.SetActive(which);
		nursePointerHead.SetActive(which);
	}
	//hitNumsT = hitNums;
	//var count = 0;

	// Update is called once per frame
	/*public int UpdateSorting () {
		sortingNumber +=18;
		if(sortingNumber > 5000){
			sortingNumber = 0;
		}
		return sortingNumber;
	}*/
	IEnumerator StartLevel()
	{
		yield return new WaitForSeconds(2);
		spawnPoint.Spawn();
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = false;
		playerT.gameObject.SetActive(true);
	}

	public void addBodyCount(GameObject body)
	{
		bodies.Add (body);
	
		if(bodies.Count >= maxBodyCount){

			Destroy(bodies[0]);
			bodies.RemoveAt(0);
		}


	}

	public void enableSpawn(SpawnPoint sp)
	{
		foreach(SpawnPoint s in spawnPoints) {
			if(s){
				s.DisableSpawn();
			}
		}
		//sp.EnableSpawn ();
		spawnPoint = sp;
	}

	void findSpawnPoints()
	{
		//int i = 0;
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("SpawnPoint")) {
			var s = go.GetComponent<SpawnPoint>();
			spawnPoints.Add (s);

			if(s.selected){
				spawnPoint = s;
				go.transform.Find ("light").gameObject.SetActive(true);
			}
			//i++;
		}
	}
	void makeHitNums()
	{
		for( int i = 0; i < maxBodyCount*2;i++) {

			GameObject hn = (GameObject)Instantiate(hitNumRef, transform.position, Quaternion.identity);
			hn.GetComponent<MeshRenderer>().sortingLayerName = "UI";
			HitNumClass hnc = new HitNumClass(hn,hn.transform,hn.GetComponent<TextMesh>());
			hitNums.Add(hnc);
		}
	}
	public void makeHitNum(Vector2 pos,float dmg)
	{
		var hnc = hitNums[hitNumCount];
		hnc.hitNumT.position = pos;
		hnc.hitNumGO.SetActive(true);
		hnc.textMesh.color = enemyHitNumClr;

		hnc.hitNumT.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100,100),50));

		if(dmg == float.MaxValue){dmg = 1000f;}
		hnc.textMesh.text = dmg.ToString("0.#");
		StartCoroutine("killHitNum",hnc.hitNumGO);
		hitNumCount++;
		if(hitNumCount == hitNums.Count){
			hitNumCount = 0;
		}
	}
	public void makePlayerHitNum(Vector2 pos,float dmg)
	{
		var hnc = hitNums[hitNumCount];
		hnc.hitNumT.position = pos;
		hnc.hitNumGO.SetActive(true);
		hnc.textMesh.color = playerHitNumClr;

		hnc.hitNumT.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100,100),50));
		
		if(dmg == float.MaxValue){dmg = 1000f;}
		hnc.textMesh.text = dmg.ToString("0.#");
		StartCoroutine("killHitNum",hnc.hitNumGO);
		hitNumCount++;
		if(hitNumCount == hitNums.Count){
			hitNumCount = 0;
		}
	}
	IEnumerator killHitNum(GameObject hn)
	{
		yield return new WaitForSeconds(1);
		hn.SetActive(false);
	}

	public void makeTeleFlash(Vector2 pos)
	{
		GameObject tf = (GameObject)Instantiate(teleFlashRef,pos, Quaternion.identity);
        Destroy (tf,1f);
	}

	public void makeHitFX(Vector2 pos)
	{
		Instantiate(hitFXref,pos, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
		
	}

	public float GetPlayerX()
	{
		return playerT.position.x;
	}
	public Transform GetPlayerTransform()
	{
		return playerT;
	}
	/*public Vector2 GetPlayerXY()
	{
		return playerT.position;
	}*/

	public class HitNumClass
		
	{

		public TextMesh textMesh;
		public Transform hitNumT;
		public GameObject hitNumGO;
		public HitNumClass(GameObject go,Transform hnT,TextMesh tm)
		{
			hitNumGO = go;
			textMesh = tm;
			hitNumT = hnT;

		}
	}
	public void StopPlayerGrab()
	{
		playerCS.gunCS.unGrab();
		playerCS.gunCS.ReleaseEnemy();

	}
	public void PausePlayer()
	{

		playerT.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		playerCS.gunCS.enabled = false;
		playerCS.enabled = false;
		playerT.GetComponent<Animator>().Play ("Idle");
		playerT.GetComponent<Animator>().SetFloat ("Speed",0);
	}
	public void unPausePlayer()
	{

		playerCS.gunCS.enabled = true;
		playerCS.enabled = true;
	}
	public void KillPlayer()
	{
		print ("kill player");
		camCS.enabled = false;
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = true;

		playerCS.gunCS.fixGuns();//ouch
		playerCS.gunCS.gameObject.SetActive(false);

		//player.GetComponent<Animator>().enabled = true;
		//anim.SetBool ("Shooting",false);
		//player.anim.ResetTrigger("Shoot");
		//player.GetComponent<Animator>().Play ("HeadIdle");//SetBool ("Shooting",false);
		GameObject go = (GameObject)Instantiate(heroDead, playerT.position, transform.rotation);
		var trq = Random.Range(3500,8000);
		if(Random.Range(0,10) > 5){
			trq = -trq;
		}
		go.transform.Find("ragdoll").Find ("psycho bob_body").GetComponent<Rigidbody2D>().AddTorque(trq);
		//Destroy(go,4);
		// ... destroy the player.
		//Destroy (col.gameObject);
		playerT.gameObject.SetActive(false);
		// ... reload the level.
		StartCoroutine("ReloadGame");
		lives--;
		livesTxt.text = lives.ToString();

	}

	IEnumerator ReloadGame()
	{		
		yield return new WaitForSeconds(3);
		if(lives>0){
			// ... pause briefly
			
			//playerDead = false;
			// ... and then reload the level.
			camCS.enabled = true;
			camCS.deathPan = true;

			playerCS.gunCS.gameObject.SetActive(true);
			playerT.gameObject.SetActive(true);
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>().enabled = false;
			
			playerT.GetComponent<PlayerHealth>().health = 100f;
			//GameObject.FindGameObjectWithTag("HealthBar").SetActive(true);
			playerT.GetComponent<PlayerHealth>().UpdateHealthBar();
			Vector3 spawnSpot = spawnPoint.gameObject.transform.Find("spawnLoc").transform.position;
			spawnSpot.z = 0f;
			playerT.transform.position = spawnSpot;
			
			playerCS.enabled = false;
			playerT.GetComponent<Rigidbody2D>().gravityScale = 0f;

		}else{
			//Application.LoadLevel(Application.loadedLevel);
			Cursor.visible = true;
			deathPanel.SetActive(true);
		}
	}
	public void LoadMenu()
	{
		Application.LoadLevel("MainMenu");
	}
	public void EnableOptions()
	{
		optionsPanel.SetActive(true);
		pauser.Pause();
	}
	public void DisableOptions()
	{
		optionsPanel.SetActive(false);
		pauser.Unpause();
	}
	public void ReloadLevel()
	{
		Cursor.visible = false;
		Application.LoadLevel(Application.loadedLevel);
	}
	public void WinLevel()
	{
		playerCS.enabled = false;
		nurse.GetComponent<Nurse>().enabled = false;
		nurse.GetComponent<Rigidbody2D>().isKinematic = true;
		nurse.GetComponent<Animator>().enabled= false;//Play ("Nurse_Idle");
		nurse.transform.Find("nurse_body").GetComponent<AudioSource>().enabled = false;
		var pos = camCS.transform.position;
		pos.z = -25;
		camCS.transform.position = pos;
		camCS.levelZoom = 0;
		Cursor.visible = false;
		StartCoroutine(WinLevel2());
		//GameObject.Find("SPAWN").SetActive(false);


		/*for (int i = enemies.Count-1; i >= 0; i--)
		{
			print (enemies.Count+"  "+enemies[i].gameObject.activeSelf);
			var e = enemies[i];
			if(e.gameObject.activeSelf){
				e.BlownUp(true,20f,transform.position);
			}
		}*/

	}
	IEnumerator WinLevel2()
	{
		yield return new WaitForSeconds(2);
		MainMenu.instance.CompleteLevel();
		nurse.SetActive(false);
		playerT.gameObject.SetActive(false);
		playerCS.gunCS.gameObject.SetActive(false);
		camCS.GetComponent<AudioListener>().enabled = true;
		winPanel.SetActive(true);
	}
	public void NextLevel()
	{
		MainMenu.instance.PlayLevel();
	}
}


