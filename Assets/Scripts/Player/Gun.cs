using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gun : MonoBehaviour
{

	//private bool shooting = false;
	public GameObject[]projectiles; //bullet/rocket/grenade etc refs
	public GameObject[] bulletShells;//bullet casings/shotgun shells
	public GameObject[] gunClips; //ammo clips(magazines) dropped when reloading

	private GameObject weaponWheel;

	public GameObject pickupCrate;   //for spawning ammo

	public float speed = 20f;				// The speed the rocket will fire at.

	private float bulletSpeed = 60f;
	private float autoCount;			//time between auto shots
	//private float maxAutoCount = 0.35f;

	private float fireSpeed = 7.5f;

	private Transform playerT;
	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private PlayerHealth playerHealth;
	private KickHit kickHit; //kickhit object

	private GameObject grabEnemyHitGO;
	//private GrabEnemyHit gehCS;
	private HingeJoint2D grabEnemyJoint;

	private Animator anim;					// Reference to the Animator component.
	private Animator playerAnim;
	//private string gunMode ="handGun";
	public enum GunMode
	{
		UNARMED,
		HANDGUN,
		MACHINEGUN,
		GRENADE,
		ROCKET,
		FLAME,
		PUNCH,
		SHOTGUN,
		STRAT

	}
	private GunMode gunMode = GunMode.UNARMED;
	private GunMode lastGunMode = GunMode.HANDGUN;
	private GunMode[] gunModes = new GunMode[9];
	//private Transform bazooka;
	private Transform head;				//hero head
	private GameObject psychoEye;



	//private GameObject unarmLGO;
	//private GameObject unarmRGO;//arms that arent attached to gun for unarmed
	private SpriteRenderer fArmR_SR;//sprites to swtich depth when switching gun left / right
	private SpriteRenderer fArmL_SR;
	private SpriteRenderer armL_SR;

	//gun references for turning on/off each weapon
	private GameObject handGunGO;
	private GameObject machineGunGO;
	private GameObject bazookaGO;
	private GameObject rpgGO;  //prop rocket for bazooka
	private GameObject punchGO;
	private GameObject flameThrowerGO;
	private GameObject shotGunGO;
	private GameObject stratGO;

	Transform bulletShell_ShotPos;//where shells come out for all weapons


	private Light gunFlareLight;//FX for each gun 
	//private Light gunFlareLight2;
	private ParticleSystem gunFlare;

	private Light handGunFlareLight;
	//private Light handGunFlareLight2;
	private ParticleSystem handGunFlare;

	private Light shotGunFlareLight;
	//private Light shotGunFlareLight2;
	private ParticleSystem shotGunFlare;

	private ParticleSystem flame;
	private ParticleSystem smoke;
	private Light flameLight;
	//private Sprite m4;
	private ParticleSystem rpgSmoke;

	private GameObject stratLightning;
	private Lightning lightningCS;

	public bool facingRight = true; //!!!!!!!!!!! 

	//private bool swungHammer = false;
	private bool canShoot = true;
	private bool canKick  = true;
	private bool reloading = false;
	//private float swungCount = 0f;

	private Weapon activeWeapon;//which weapon object is selected

	private Weapon handGunWeapon;//each weapon class with stats/data
	private Weapon machineGunWeapon;
	private Weapon grenadeWeapon;
	private Weapon flameWeapon;
	private Weapon rpgWeapon;
	private Weapon punchWeapon;
	private Weapon shotGunWeapon;
	private Weapon stratWeapon;

	private Weapon[] weapons = new Weapon[9];//array of Weapon classes
	private Weapon[] weaponsForAmmo = new Weapon[6]; //array of weapons for spawning ammo/health

	private float dir = 180f;

	public GameObject xhairRef;// cross hair spawn
	private Transform xhair;// actual crosshair
	private Transform xhairHUD;//UI crosshair

	private UI_WeaponDisplay hudGun;
	private Text ammoDisp;

	private AudioSource aud; //sound FX
	public AudioClip []hgSFX;
	public AudioClip[] mgSFX;
	public AudioClip bSFX;
	public AudioClip fSFX;  //flame start sound on normal audio
	private AudioSource flameAud; //flame on sfx on flamethrowers own audio source
	public AudioClip[] rpgSFX;
	public AudioClip[] sgSFX;
	public AudioClip[] stratSFX;

	public AudioClip[] kickSFX;
	public AudioClip[] yellSFX;



	private SpriteRenderer headSR;//????? head Sprites
	public Sprite[] headS;

	private Transform[] armTargets = new Transform[8];//active arm targets on gun for IK
	private SimpleCCD[] armIK = new SimpleCCD[2];

	//private Transform[] elbowTargets = new Transform[2];

	//where all the hands are on each gun
	private Vector3[] armTargetPos = new Vector3[8];//arm target positions for all guns

	public AudioClip[] reloadFX;

	public float[] cameraXmaxMin = new float[2];


	private Vector3 mousePos;
	private Vector3 angle;
	//private Transform dummyCam;
	//private Vector3 oldMousePos = Vector3.zero;
	void Awake()
	{
		anim = GetComponent<Animator>();

		aud = transform.Find ("weapon Transform").Find("Audio").GetComponent<AudioSource>();
		//aud.clip = hgSFX;
		ammoDisp = GameObject.Find("ammoClipTXT").GetComponent<Text>();
		// Setting up the references.

		playerT = GameObject.FindGameObjectWithTag("Player").transform;
		playerCtrl = playerT.GetComponent<PlayerControl>();
		playerHealth = playerT.GetComponent<PlayerHealth>();

		//print (playerT.name);

		playerCtrl.gunCS = this;
		playerAnim = playerT.GetComponent<Animator>();
		kickHit = playerT.Find ("kickHit").GetComponent<KickHit>();

		grabEnemyHitGO = playerT.Find ("grabEnemyHit").gameObject;
		//gehCS = grabEnemyHitGO.GetComponent<GrabEnemyHit> ();
		grabEnemyHitGO.GetComponent<GrabEnemyHit> ().gunCS = this;



		//yellSFX = playerCtrl.jumpFX;//make kick yell SFX same as jump from player control


		head = playerT.Find("head Transform");
		headSR = head.transform.Find ("head").GetComponent<SpriteRenderer>();

		psychoEye = head.Find("head").Find("psychoEye").gameObject;

		SliderJoint2D jnt = playerT.gameObject.AddComponent<SliderJoint2D>();
		jnt.connectedBody = GetComponent<Rigidbody2D>();
		jnt.anchor = new Vector2(0,0.6f);
		jnt.useLimits = true;
		jnt.angle = 90f;
		JointTranslationLimits2D l = new JointTranslationLimits2D();
		l.max = 1.2f;
		jnt.limits = l;

		//WEAPONS STUFF

		handGunWeapon = new Weapon(GunMode.HANDGUN,transform.Find("weapon Transform"));
		handGunWeapon.bulletShell = bulletShells [0];
		machineGunWeapon = new Weapon(GunMode.MACHINEGUN,transform.Find("weapon Transform"));
		machineGunWeapon.bulletShell = bulletShells [0];
		grenadeWeapon = new Weapon (GunMode.GRENADE,transform.Find("weapon Transform"));
		flameWeapon = new Weapon (GunMode.FLAME,transform.Find("weapon Transform"));
		rpgWeapon = new Weapon (GunMode.ROCKET,transform.Find("weapon Transform"));
		punchWeapon = new Weapon (GunMode.PUNCH,transform.Find("weapon Transform"));
		shotGunWeapon = new Weapon (GunMode.SHOTGUN,transform.Find("weapon Transform"));
		shotGunWeapon.bulletShell = bulletShells [1];
		stratWeapon = new Weapon (GunMode.STRAT,transform.Find("weapon Transform"));

		activeWeapon = handGunWeapon;

		weapons[0]=new Weapon(GunMode.UNARMED,transform.Find("weapon Transform"));
		weapons[1]=handGunWeapon;
		weapons[2]=machineGunWeapon;
		weapons[3]=grenadeWeapon;
		weapons[4]=flameWeapon;
		weapons[5]=rpgWeapon;
		weapons[6]=punchWeapon;
		weapons [7] = shotGunWeapon;
		weapons[8] = stratWeapon;

		weaponsForAmmo[0] = machineGunWeapon;
		weaponsForAmmo[1] = grenadeWeapon;
		weaponsForAmmo[2] = flameWeapon;
		weaponsForAmmo[3] = rpgWeapon;
		weaponsForAmmo[4] = punchWeapon;
		weaponsForAmmo [5] = shotGunWeapon;

		gunModes[0]=GunMode.UNARMED;
		gunModes[1]=GunMode.HANDGUN;
		gunModes[2]=GunMode.MACHINEGUN;
		gunModes[3]=GunMode.GRENADE;
		gunModes[4]=GunMode.FLAME;
		gunModes[5]=GunMode.ROCKET;
		gunModes[6]=GunMode.PUNCH;
		gunModes[7]=GunMode.SHOTGUN;
		gunModes[8]=GunMode.STRAT;

		//unarmLGO = playerT.Find ("body").Find("psycho bob_armL").gameObject;//unarmed arms, animated separately
		//unarmRGO = playerT.Find ("body").Find("psycho bob_armR").gameObject;

		handGunGO = transform.Find ("weapon Transform").Find ("handGun3D").gameObject;
		bazookaGO = transform.Find ("weapon Transform").Find ("bazooka3D").gameObject;
		machineGunGO = transform.Find ("weapon Transform").Find ("machineGun3D").gameObject;
		flameThrowerGO = transform.Find ("weapon Transform").Find ("flameThrower3D").gameObject;
		shotGunGO = transform.Find ("weapon Transform").Find ("shotGun3D").gameObject;
		stratGO  = transform.Find ("weapon Transform").Find ("stratGun3D").gameObject;
		rpgGO    = transform.Find ("weapon Transform").Find("bazooka3D").Find("rocket3D").gameObject;
		punchGO  = transform.Find ("weapon Transform").Find("bazooka3D").Find("punch3D").gameObject;

		weaponWheel = transform.Find("UIweaponWheel").gameObject;//GameObject.Find ("UI").transform.Find("UIweaponWheel").gameObject;//playerT.Find("weaponSelect").gameObject;
		weaponWheel.transform.SetParent (GameObject.Find ("UI").transform, false);


		bulletShell_ShotPos =transform.Find ("weapon Transform"). transform.Find ("bulletShell_spot");

		handGunFlare = transform.Find ("weapon Transform").Find ("handGun3D").Find ("handGunFlare").GetComponent<ParticleSystem>(); 
		handGunFlareLight = transform.Find ("weapon Transform").Find ("handGun3D").Find("handGunFlare light").GetComponent<Light>();
		//handGunFlareLight2 = transform.Find ("weapon Transform").Find ("handGun3D").Find("handGunFlare light2").GetComponent<Light>();


		gunFlare = transform.Find ("weapon Transform").Find ("machineGun3D").Find ("gunFlare").GetComponent<ParticleSystem>(); 
		gunFlareLight = transform.Find ("weapon Transform").Find ("machineGun3D").Find("gunFlare light").GetComponent<Light>();
		//gunFlareLight2 = transform.Find ("weapon Transform").Find ("machineGun3D").Find("gunFlare light2").GetComponent<Light>();

		shotGunFlare = transform.Find ("weapon Transform").Find ("shotGun3D").Find ("gunFlare").GetComponent<ParticleSystem>(); 
		shotGunFlareLight = transform.Find ("weapon Transform").Find ("shotGun3D").Find("gunFlare light").GetComponent<Light>();
		//shotGunFlareLight2 = transform.Find ("weapon Transform").Find ("shotGun3D").Find("gunFlare light2").GetComponent<Light>();

		flame = transform.Find ("weapon Transform").Find ("flameThrower3D").Find("flame").GetComponent<ParticleSystem>();
		smoke = transform.Find ("weapon Transform").Find ("flameThrower3D").Find("flameSmoke").GetComponent<ParticleSystem>();
		flameLight =  transform.Find ("weapon Transform").Find ("flameThrower3D").Find("flameFX").GetComponent<Light>();
		flameAud = flameLight.gameObject.GetComponent<AudioSource>();

		rpgSmoke = transform.Find ("weapon Transform").Find ("bazooka3D").Find("rpgSmoke").GetComponent<ParticleSystem>();

		stratLightning = transform.Find ("lightning").gameObject;
		stratLightning.transform.parent = null;
		lightningCS = stratLightning.GetComponent<Lightning> ();
		stratLightning.SetActive(false);

		armIK[0] = playerT.Find ("body").Find ("psycho bob_armL").GetComponent<SimpleCCD>();
		armIK[1] = playerT.Find ("body").Find("psycho bob_armR").GetComponent<SimpleCCD>();

		armTargets[0]= transform.Find("weapon Transform").Find ("fArmB_L_IK");//for flipping when gun flips
		armTargets[1]= transform.Find("weapon Transform").Find ("fArmB_R_IK");
		armTargets[2]= playerT.Find ("handL_IK");//for unarmed IK targets
		armTargets[3]= playerT.Find ("handR_IK");
		armTargets[4]= stratGO.transform.Find("handLmg");//for strat
		armTargets[5]= stratGO.transform.Find ("handRmg");
		armTargets[6]= playerT.Find ("handL_IK_grab"); //for grabbing enemies;
		armTargets[7]= playerT.Find ("handR_IK_grab");


		fArmR_SR = armIK[1].transform.Find("psycho bob_fArmR").GetComponent<SpriteRenderer>();
		fArmL_SR = armIK[0].transform.Find("psycho bob_fArmL").GetComponent<SpriteRenderer>();
		armL_SR = armIK[0].transform.parent.Find("psycho bob_armL").GetComponent<SpriteRenderer>();

		//elbowTargets [0] = playerT.Find ("body").Find ("Skeleton").Find ("armB_L_IK");
		//elbowTargets [1] = playerT.Find ("body").Find ("Skeleton").Find ("armB_R_IK");

		armTargetPos[0] = armTargets[0].localPosition;//handgun machine gun L
		armTargetPos[1] = armTargets[1].localPosition;//R
		armTargetPos[2] = new Vector3(-0.8f,-0.4f,armTargets[0].localPosition.z);//flamethrower
		armTargetPos[3] = new Vector3(1.2f,-0.2f,armTargets[1].localPosition.z);
		armTargetPos[4] = new Vector3(-0.6f,-0.1f,armTargets[1].localPosition.z);//bazooka
		armTargetPos[5] = new Vector3(2f,1.57f,armTargets[0].localPosition.z);
		armTargetPos[6] = new Vector3(-.75f,-0.3f,armTargets[0].localPosition.z);//shotgun
		armTargetPos[7] = new Vector3(1.5f,0.5f,armTargets[0].localPosition.z);
		//armTargetPos[8] = new Vector3(-1f,0.8f,armTargets[0].localPosition.z);//strat
		//armTargetPos[9] = new Vector3(1.8f,-0.5f,armTargets[0].localPosition.z);

		hudGun =GameObject.FindGameObjectWithTag("HUD").transform.Find ("gunHUD").Find ("weaponIMG").GetComponent<UI_WeaponDisplay>();

		//hero = transform.root.gameObject.transform;
		bazookaGO.SetActive(false);
		machineGunGO.SetActive(false);
		//flameThrowerGO.SetActive(false);
		//camera = Camera.main;
		var xhairGO = (GameObject)Instantiate (xhairRef,transform.position  , Quaternion.identity);
		xhair = xhairGO.transform;
		xhair.name = "xhair";
		Cursor.visible = false;

		xhairHUD =GameObject.FindGameObjectWithTag("HUD").transform.Find ("xhairHUD");

		gunMode = GunMode.UNARMED;

		SwitchWeapons();

		cameraXmaxMin = Camera.main.transform.GetComponent<CameraFollow>().GetCameraMaxMin();

		//dummyCam = Camera.main.transform.Find ("dummyCam");
	}
	void FixedUpdate()
	{
		Vector3 mouse = Input.mousePosition;
		xhairHUD.position = mouse;
		mouse.z = -Camera.main.transform.position.z;
		
		mousePos =Camera.main.ScreenToWorldPoint (mouse);
		mousePos.z = -2f;
		//Vector3 v = new Vector3(mousePos.x,mousePos.y,-2f);
		if (Vector3.Distance(mousePos,xhair.position)> 0.05f){
			xhair.position = Vector3.Lerp(xhair.transform.position,mousePos,20f * Time.deltaTime);


		}

		//oldMousePos = mousePos;
	}

	void Update ()
	{
		//transform.position = gunSpot.position;
		//get the xhair position based on mouse xy and camera z

		//check max min camera vals
		/*if(mousePos.x>cameraXmaxMin[0]){
			mousePos.x = cameraXmaxMin[0];
		}else if(mousePos.x<cameraXmaxMin[1]){
			mousePos.x = cameraXmaxMin[1];
		}*/


		Vector3 charPos = transform.position;

		float diffX = mousePos.x - charPos.x;
		float diffY = mousePos.y - charPos.y;
	
		float angle2 = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
		Quaternion angle3 = Quaternion.Euler(new Vector3(0, 0, angle2));
		Quaternion headAngle = angle3;
		Quaternion angleRot = angle3;



		//if(canShoot){
			if(facingRight){
				
				//firing backwards? flip gun scaleY
				if(mousePos.x < playerT.position.x){
					//print ("FLIIIIPS"+facingRight);
					Flip ();
				}
			}else{
				//going forward agian , gun still slip scaleY>?
				if(mousePos.x > playerT.position.x){
					//print ("FLIIIIPS"+facingRight);
					Flip ();
				}
			}

		//Debug.DrawRay(transform.position, new Vector2(mousePos.x,mousePos.y), Color.white,1);
			if(!playerCtrl.facingRight){
				angle2 = Mathf.Atan2(diffY, diffX * -1) * Mathf.Rad2Deg;
				headAngle = Quaternion.Euler(new Vector3(0, 0, angle2));
			}
			transform.rotation = angle3;
			head.rotation = headAngle;
		//}
		

		angle = (mousePos - charPos).normalized;
		//print ("ud "+angle);

		// If the fire button is pressed...
		if (Input.GetButtonDown ("Fire1")) {
						// If the player is facing right...
			dir = 180f;
			if (playerCtrl.facingRight) {	
					dir = 0;
			}
			if(gunMode == GunMode.UNARMED && canShoot){

					StartCoroutine("Grab");


			}else if(gunMode == GunMode.HANDGUN && canShoot){

				AudioSource.PlayClipAtPoint(hgSFX[UnityEngine.Random.Range (0, hgSFX.Length)], aud.transform.position);
				//aud.Play ();
				
				anim.SetTrigger ("Shoot");
				playerAnim.SetTrigger("Shoot");
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = (GameObject)Instantiate (projectiles[0],activeWeapon.shotPos.position, angleRot);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				//bulletInstance.GetComponent<Rigidbody2D>().transform.rotation = angleRot;

				handGunFlare.Emit (1);
				handGunFlareLight.enabled = true;
				//handGunFlareLight2.enabled = true;
				StartCoroutine(handGunflareLight());

				canShoot = false;

				makeBulletShell();


				if(!playerCtrl.grounded){
					//print (angle);
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 500f);
				}
				activeWeapon.clip--;
				if(activeWeapon.clip == 0){
					Reload();
				}else{
					StartCoroutine(canShootAgain(0.2f));
				}



			}else if(gunMode ==GunMode.MACHINEGUN && canShoot){
				AudioSource.PlayClipAtPoint(mgSFX[UnityEngine.Random.Range (0, mgSFX.Length)], aud.transform.position);
				//aud.Play ();
				anim.SetTrigger ("Shoot");
				playerAnim.SetTrigger("Shoot");
				GameObject bullet2 = (GameObject)Instantiate(projectiles[1], activeWeapon.shotPos.position, angleRot);
				bullet2.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				//bullet2.GetComponent<Rigidbody2D>().transform.rotation = angleRot;

				anim.SetBool ("shooting",true);
				gunFlare.Emit (100);
				gunFlareLight.enabled = true;
				//gunFlareLight2.enabled = true;
				StartCoroutine(flareLight());
				autoCount = Time.time;
				makeBulletShell();
				if(!playerCtrl.grounded){
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 250f);
				}
				activeWeapon.clip--;
				if(activeWeapon.clip == 0){
					Reload();
				}
			
			}else if(gunMode == GunMode.GRENADE && canShoot){
				aud.Play();
				anim.SetTrigger ("Shoot");
				playerAnim.SetTrigger("Shoot");
				GameObject grenade2 = (GameObject)Instantiate(projectiles[2], activeWeapon.shotPos.position, angleRot);
				//grenade2.GetComponent<Rigidbody2D>().transform.rotation = angleRot;
				float force = (Mathf.Abs(diffX) + Mathf.Abs(diffY))*80f;
				if(force<1300f){force=1300f;}
				grenade2.GetComponent<Rigidbody2D>().AddForce(new Vector2 (angle.x, angle.y) * force);
				grenade2.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0f,-50f));

				//canShoot = false;
				//StartCoroutine(canShootAgain(1.5f));
				if(!playerCtrl.grounded){
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 750f);
				}
				activeWeapon.clip--;
				Reload ();
			}else if(gunMode == GunMode.FLAME && canShoot){
				playerAnim.SetTrigger("Shoot");
				aud.Play ();
				flameAud.enabled = true;
				flame.Play();
				smoke.Play();
				flameLight.enabled = true;
				autoCount = Time.time;
				GameObject bullet2 = (GameObject)Instantiate(projectiles[3], activeWeapon.shotPos.position, Quaternion.identity);
				bullet2.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * fireSpeed;
				flameLight.enabled = true;
				flameLight.intensity = 0f;
				CancelInvoke();
				InvokeRepeating("flameFXOn",0f,0.1f);
				activeWeapon.clip--;
				if(activeWeapon.clip == 0){
					Reload();
				}
			}else if(gunMode == GunMode.ROCKET && canShoot){
				aud.clip = rpgSFX[UnityEngine.Random.Range (0, rpgSFX.Length)];
				aud.Play();
				anim.SetTrigger ("Shoot");
				playerAnim.SetTrigger("Shoot");
				rpgGO.SetActive(false);
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = (GameObject)Instantiate (projectiles[4],activeWeapon.shotPos.position , angleRot);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * 40;
				//bulletInstance.GetComponent<Rigidbody2D>().transform.rotation = angleRot;
				rpgSmoke.Play ();//Emit (1000);
				
				//canShoot = false;
				//StartCoroutine(canShootAgain(2.25f));
				if(!playerCtrl.grounded){
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 1000f);
				}
				activeWeapon.clip--;
				Reload ();
			}else if(gunMode == GunMode.PUNCH && canShoot){
				aud.Play();
				anim.SetTrigger ("Shoot");
				playerAnim.SetTrigger("Shoot");
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = (GameObject)Instantiate (projectiles[5],activeWeapon.shotPos.position, angleRot);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * 50f;
				//bulletInstance.GetComponent<Rigidbody2D>().transform.rotation = angleRot;
				Destroy(bulletInstance,3f);
				punchGO.SetActive(false);
				rpgSmoke.Play ();//Emit (1000);
				
				//canShoot = false;
				//StartCoroutine(canShootAgain(2.25f));
				if(!playerCtrl.grounded){
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 1000f);
				}
				activeWeapon.clip--;
				Reload ();
			}else if(gunMode == GunMode.SHOTGUN && canShoot){
				aud.clip = sgSFX[UnityEngine.Random.Range (0, sgSFX.Length)];
				aud.Play ();
				
				anim.SetTrigger ("ShootShotgun");
				playerAnim.SetTrigger("Shoot");
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				GameObject bulletInstance = (GameObject)Instantiate (projectiles[6],activeWeapon.shotPos.position, Quaternion.identity);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				bulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(100f,500f));
				bulletInstance = (GameObject)Instantiate (projectiles[6],activeWeapon.shotPos.position, Quaternion.identity);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				bulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(10f,300f));
				bulletInstance = (GameObject)Instantiate (projectiles[6],activeWeapon.shotPos.position, Quaternion.identity);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				bulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(-10f,-300f));
				bulletInstance = (GameObject)Instantiate (projectiles[6],activeWeapon.shotPos.position, Quaternion.identity);
				bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
				bulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(-100f,-500f));
				
				shotGunFlare.Play ();// (1000);
				shotGunFlareLight.enabled = true;
				//shotGunFlareLight2.enabled = true;
				StartCoroutine(shotGunflareLight());
				
				canShoot = false;
				

				
				
				if(!playerCtrl.grounded){
					playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 1750f);
				}
				activeWeapon.clip--;
				if(activeWeapon.clip == 0){
					Reload();
				}else{
					StartCoroutine(canShootAgain(0.75f));
				}
			}else if(gunMode == GunMode.STRAT && canShoot){
				//print ("SHOOT STRAT");
				anim.SetTrigger("ChargeStrat");
				//autoCount = Time.time;
				//GameObject bullet2 = (GameObject)Instantiate(projectiles[3], activeWeapon.shotPos.position, Quaternion.identity);
				//bullet2.rigidbody2D.velocity = new Vector2 (angle.x, angle.y) * fireSpeed;
				anim.SetBool("shooting",true);
				playerAnim.Play ("heroChargeStrat");
				aud.clip = stratSFX[UnityEngine.Random.Range (0, stratSFX.Length-1)];
				aud.Play();
				stratGO.GetComponent<AudioSource>().Play();//enabled = true;
				activeWeapon.chargeCount = 0;
				//canShoot = false;
			}
			UpdateAmmoDisp();
	///SWITCH WEAPONS
		} else if (Input.GetButtonDown ("Fire2")) {//weapon select pressed

			Time.timeScale = 0;
			Vector3 theScale = weaponWheel.transform.localScale;

			if (playerCtrl.facingRight) {	

				theScale.x =1;

			}else{
				theScale.x = -1;
			}
			weaponWheel.transform.localScale = theScale;
			weaponWheel.SetActive(true);
			//InvokeRepeating("WeaponWheelCursor",0,0.1f);
		
		}

		//fire button down
		if(Input.GetButton ("Fire1")){
			if (gunMode == GunMode.MACHINEGUN && canShoot) {
				//Debug.Log ("delta time "+Time.deltaTime+"  "+autoCount);
				if(Time.time - autoCount >= 0.15f)//autoCount < maxAutoCount){
				{
					AudioSource.PlayClipAtPoint(mgSFX[UnityEngine.Random.Range (0, mgSFX.Length)], aud.transform.position);
					//aud.clip = mgSFX[UnityEngine.Random.Range (0, mgSFX.Length)];
					//aud.Play ();
					//Debug.Log("shooting");
					autoCount = Time.time;
					GameObject bullet2 = (GameObject)Instantiate(projectiles[1], activeWeapon.shotPos.position, angleRot);
					bullet2.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * bulletSpeed;
					//bullet2.GetComponent<Rigidbody2D>().transform.rotation = angleRot;
			
					anim.SetTrigger ("Shoot");
					playerAnim.SetTrigger("Shoot");
					//aud.Play ();
					gunFlareLight.enabled = true;
					//gunFlareLight2.enabled = true;
					StartCoroutine(flareLight());
					gunFlare.Emit (100);
					makeBulletShell();
					if(!playerCtrl.grounded){
						playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * 250f);
					}
					activeWeapon.clip--;
					if(activeWeapon.clip == 0){
						Reload();
					}
					UpdateAmmoDisp();
				}
			}else if(gunMode == GunMode.FLAME){
				if(Time.time - autoCount >= 0.2f)//autoCount < maxAutoCount){
				{
					//Debug.Log("shooting");
					autoCount = Time.time;
					GameObject bullet2 = (GameObject)Instantiate(projectiles[3], activeWeapon.shotPos.position, Quaternion.identity);
					bullet2.GetComponent<Rigidbody2D>().velocity = new Vector2 (angle.x, angle.y) * fireSpeed;
					activeWeapon.clip--;
					if(activeWeapon.clip == 0){
						Reload();
					}
					UpdateAmmoDisp();
				}
			}/*else if (gunMode == GunMode.STRAT && canShoot) {
				if(Time.time - autoCount >= 1f)//autoCount < maxAutoCount){
				{
					autoCount = Time.time;
					activeWeapon.chargeCount++;
					if(activeWeapon.chargeCount ==6){

					}
					print (activeWeapon.chargeCount);
				}
			}*/
		}//else if(Input.GetButton ("Fire2")){ /// weapon select button down
			//SelectWeapons(mousePos);
		//}


		//BUTTON UNPRESSED
		if (Input.GetButtonUp ("Fire1")) {
			anim.SetBool ("shooting", false);
			if(gunMode == GunMode.UNARMED){
				unGrab();
				if (grabEnemyJoint) {
					ThrowEnemy();
				}
			}else if(gunMode == GunMode.STRAT){
				ShootStrat(angle,angleRot);
			}else if(gunMode == GunMode.FLAME){
				flame.Stop ();
				smoke.Stop ();
				InvokeRepeating("flameFXOff",0f,0.1f);
			}
		}else if(Input.GetButtonUp ("Fire2")){
			weaponWheel.SetActive(false);

			SwitchWeapons();

			Time.timeScale = 1;
		}
		//RELOAD
		if (Input.GetButtonDown("Reload") && activeWeapon.clip < activeWeapon.maxClip) {

				Reload();

		}else if (Input.GetButtonDown("Kick") && canKick && playerCtrl.CanKick()){
			//anim.SetBool ("shooting", false);
			playerAnim.Play ("Kick");

			StartCoroutine("Kick");
		}

		//else if (Input.GetKeyDown(KeyCode.T)) {//                                REMOVE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//SpawnPickup(new Vector3(transform.position.x + 5f,transform.position.y,transform.position.z));
		//}
	}



	IEnumerator Kick()
	{
		if (grabEnemyJoint) {
			ReleaseEnemy();
		}
		canKick = false;
		AudioSource.PlayClipAtPoint(kickSFX[UnityEngine.Random.Range (0, kickSFX.Length)], transform.position);
		
		
		yield return new WaitForSeconds(0.15f);
		
		/*float dirr = Input.GetAxis ("Horizontal");//-1f;
		if(Mathf.Abs(dirr) == 0f){//if not running still give it a bit of x speed in the dir theyre facing.  the way the enemies fly when hit depends on the speed of kickHit ball

			dirr = -0.2f;
			if(playerCtrl.facingRight){
				dirr = 0.2f;
			}
		}*/
		int i = UnityEngine.Random.Range (0, yellSFX.Length);
		AudioSource.PlayClipAtPoint(yellSFX[i], transform.position);
		//Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		kickHit.gameObject.SetActive(true);
		kickHit.KickStart();
		//GameObject kh = (GameObject)Instantiate(projectiles[8], kickHitSpot.position, randomRotation);
		//kh.GetComponent<Rigidbody2D>().velocity = new Vector2(dirr*1.5f,2.5f);
		yield return new WaitForSeconds(0.2f);
		kickHit.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		canKick = true;
	}

	private IEnumerator Grab()
	{
		canShoot = false;
		grabEnemyHitGO.SetActive (true);
		playerAnim.Play ("TryGrab");
		yield return new WaitForSeconds (1);
		canShoot = true;
		grabEnemyHitGO.SetActive (false);
	}
	public void unGrab()
	{
		//return;
		canShoot = true;
		StopCoroutine ("GrabEnemy");
		grabEnemyHitGO.SetActive (false);
		
	}
	public void GrabEnemy(Rigidbody2D rb)
	{
		grabEnemyJoint = playerT.gameObject.AddComponent<HingeJoint2D> ();
		grabEnemyJoint.connectedBody = rb;
		grabEnemyJoint.anchor = new Vector2 (2, 3.2f);
		grabEnemyJoint.connectedAnchor = new Vector2 (0, 1.7f);
		armIK[0].target = armTargets[6];
		armIK[1].target = armTargets[7];
	}
	public void ReleaseEnemy()
	{
		var rb = grabEnemyJoint.connectedBody;
		Destroy (grabEnemyJoint);
		var rdCS = rb.transform.root.GetComponent<Enemy_Ragdoll>();
		rdCS.makeNormalHeavy();
		rdCS.grabbed = false;
		armIK[0].target = armTargets[2];
		armIK[1].target = armTargets[3];
	}

	private void ThrowEnemy()
	{

		var rb = grabEnemyJoint.connectedBody;
		Destroy (grabEnemyJoint);
		var rdCS = rb.transform.root.GetComponent<Enemy_Ragdoll>();
		rdCS.makeNormalHeavy();
		rdCS.grabbed = false;
		armIK[0].target = armTargets[2];
		armIK[1].target = armTargets[3];

		rb.velocity = new Vector2 (angle.x, angle.y) * 300;


		
	}
	private void ChargeStrat()//called by animator
	{

		activeWeapon.chargeCount++;
		UpdateAmmoDisp();
		//print ("charge " + activeWeapon.chargeCount);
	}
	private void ShootStrat(Vector3 angle, Quaternion angleR)
	{
		//print (activeWeapon.chargeCount);

		stratGO.GetComponent<AudioSource>().Stop();
		anim.SetBool ("shooting", false);

		if (activeWeapon.chargeCount <= 0) {
			aud.Stop();
			playerAnim.SetTrigger("stopCharge");
			return;
		}else{//iff its a false start (0 strat charge) let him shoot again
			canShoot = false;
			StartCoroutine(canShootAgain(1f));
		}
			
		playerAnim.Play("heroShootStrat");
		string anm = "ShootStratGun"+activeWeapon.chargeCount.ToString();
		anim.Play (anm);
		

		Vector3 pos = (activeWeapon.shotPos.position-transform.position).normalized;

		RaycastHit2D hit;
		Vector3 length = activeWeapon.shotPos.position+(pos*50f);
		//Debug.DrawLine(activeWeapon.shotPos.position,length, Color.red,5f);
		if(hit = Physics2D.Linecast (activeWeapon.shotPos.position,length , 1 << LayerMask.NameToLayer("Ground")))
		{
			//Debug.DrawLine(activeWeapon.shotPos.position,hit.point, Color.red);
			length = hit.point;
		}
	
		GameObject lb = (GameObject)Instantiate(projectiles[7], activeWeapon.shotPos.position, angleR);
		lb.GetComponent<LaserBeam>().damage = activeWeapon.chargeCount;
		Vector3 scl = lb.transform.localScale;
		scl.x = Vector2.Distance(lb.transform.position , length);
		lb.transform.localScale =scl;
		//print (scl.x);
		aud.clip = stratSFX[4];
		aud.Play ();

		if(activeWeapon.chargeCount>4){
			stratLightning.SetActive (true);
			lightningCS.SetPoints (lb.transform.position, length); 
		}
		if(!playerCtrl.grounded){
			playerCtrl.GetComponent<Rigidbody2D>().AddForce(new Vector2 (-angle.x, -angle.y) * (500f * activeWeapon.chargeCount));
		}

		activeWeapon.chargeCount = 0;
		UpdateAmmoDisp();
	}
	private void Reload()
	{
		if(activeWeapon.ammo<=0){ //if no ammo for current gun
			SwitchToWeapon(GunMode.HANDGUN);
			return;
		}
		reloading = true;
		anim.SetBool ("shooting",false);
		canShoot = false;
		StartCoroutine ("Reloading");
		if(gunMode == GunMode.HANDGUN){

			anim.SetTrigger("ReloadHandGun");
			Instantiate(gunClips[0], transform.position, transform.rotation);
			AudioSource.PlayClipAtPoint(reloadFX[1], transform.position);
		}else if(gunMode == GunMode.MACHINEGUN){
			anim.SetTrigger("ReloadMachineGun");
			Instantiate(gunClips[1], transform.position, transform.rotation);
			AudioSource.PlayClipAtPoint(reloadFX[1], transform.position);
		}else if(gunMode == GunMode.GRENADE){
			anim.SetTrigger("ReloadGrenade");
			AudioSource.PlayClipAtPoint(reloadFX[2], transform.position);
		}else if(gunMode == GunMode.ROCKET){
			anim.SetTrigger("ReloadRPG");
			AudioSource.PlayClipAtPoint(reloadFX[2], transform.position);
		}else if(gunMode == GunMode.FLAME){
			anim.SetTrigger("ReloadFlame");
			Instantiate(gunClips[2], transform.position, transform.rotation);
			flame.Stop ();
			smoke.Stop ();
			InvokeRepeating("flameFXOff",0f,0.1f);
		}else if(gunMode == GunMode.PUNCH){
			anim.SetTrigger("ReloadPunch");
			AudioSource.PlayClipAtPoint(reloadFX[2], transform.position);
		}else if(gunMode == GunMode.SHOTGUN){
			anim.SetTrigger("ReloadShotGun");
			AudioSource.PlayClipAtPoint(reloadFX[3], transform.position);
		}

	}
	IEnumerator Reloading()
	{

		yield return new WaitForSeconds(activeWeapon.reloadTime);
		if(activeWeapon.ammo + activeWeapon.clip >= activeWeapon.maxClip){ //if they have enough in the clip + in ammo to fill the clip
			activeWeapon.ammo = activeWeapon.ammo - activeWeapon.maxClip +activeWeapon.clip; //subtract a full clip but add what he had in the clip already
			activeWeapon.clip = activeWeapon.maxClip;
		}else{                                                             //otherwise put clip + ammo in the clip and make ammo zero
			activeWeapon.clip = activeWeapon.ammo + activeWeapon.clip;
			activeWeapon.ammo =0;
		}
		if(gunMode==GunMode.ROCKET){
			rpgGO.SetActive(true);
		}else if(gunMode==GunMode.PUNCH){
			punchGO.SetActive(true);
		}
		canShoot = true;
		reloading = false;
		UpdateAmmoDisp();
	}


	 
	void UpdateAmmoDisp()
	{
		string ammo = activeWeapon.ammo.ToString();
		if(activeWeapon.ammo>1000){
			ammo = "totally";
		}
		string clip = activeWeapon.clip.ToString();
		if(gunMode == GunMode.STRAT){
			clip = activeWeapon.chargeCount.ToString();
		}
		ammoDisp.text =clip+" / "+ammo;
	}

	//SWITCHING WEAPONS

	public void SwitchWeaponMode(int mode)//used by event triggers on weapon wheel
	{
		if (weapons [mode].hasGun) {
			gunMode = gunModes [mode];
		}
	}
	public void SwitchToWeapon(GunMode gm)//to switch automatically, ie out of ammo switch to handgun
	{
		gunMode = gm;
		SwitchWeapons();
	}
	private void SwitchWeapons() 
	{
		//Color white = Color.white;
		//foreach(Transform child in weaponWheel.transform){
			//child.GetComponent<Toggle>().colors.normalColor = white;
		//}
		if (reloading) {
			anim.SetTrigger("StopReloading");
			StopCoroutine("Reloading");
			reloading = false;
			canShoot = true;
		}

		handGunGO.SetActive(false);
		machineGunGO.SetActive(false);
		flameThrowerGO.SetActive(false);
		//punchGO.SetActive(false);
		//rpgGO.SetActive(false);
		bazookaGO.SetActive(false);
		punchGO.SetActive(false);
		shotGunGO.SetActive (false);
		stratGO.SetActive(false);
		//print (gunMode);
		if(lastGunMode == GunMode.UNARMED || lastGunMode == GunMode.STRAT){
			armIK[0].transform.Find ("psycho bob_fArmL").Find ("psycho bob_handL").GetComponent<SpriteRenderer>().enabled = false;
			armIK[1].transform.Find ("psycho bob_fArmR").Find ("psycho bob_handR").GetComponent<SpriteRenderer>().enabled = false;
			armIK[0].target = armTargets[0];
			armIK[1].target = armTargets[1];
			aud.Stop();
			playerAnim.SetTrigger("stopCharge");
			stratWeapon.chargeCount = 0;
		}


		if(gunMode == GunMode.UNARMED){
			armIK[0].transform.Find ("psycho bob_fArmL").Find ("psycho bob_handL").GetComponent<SpriteRenderer>().enabled = true;
			armIK[1].transform.Find ("psycho bob_fArmR").Find ("psycho bob_handR").GetComponent<SpriteRenderer>().enabled = true;
			armIK[0].target = armTargets[2];
			armIK[1].target = armTargets[3];
			hudGun.switchWeapon(0);
			gunMode = GunMode.UNARMED;
		
		
		}else if(gunMode == GunMode.HANDGUN){                      //handGun
			//weaponWheel.transform.Find("1").GetComponent<Toggle>().isOn = true;//colors.normalColor = new Color(.6,.7,.2,1);
			//aud.clip = hgSFX;

			StopCoroutine("canShootAgain");
			canShoot = true;
			handGunGO.SetActive(true);

			gunMode = GunMode.HANDGUN;

			hudGun.switchWeapon(1);
			armTargets[0].localPosition = armTargetPos[0];
			armTargets[1].localPosition = armTargetPos[1];
			activeWeapon = handGunWeapon;

		}else if (gunMode == GunMode.MACHINEGUN){             //machine gun
			//aud.clip = mgSFX;
			canShoot = true;
			//bazookaRen.sprite = m4;
			machineGunGO.SetActive(true);

			machineGunGO.transform.Find ("handLmg").gameObject.SetActive(true);
			machineGunGO.transform.Find ("handL2mg").gameObject.SetActive(false);
			
			gunMode = GunMode.MACHINEGUN;
			hudGun.switchWeapon(2);
			armTargets[0].localPosition = armTargetPos[0];
			armTargets[1].localPosition = armTargetPos[1];
			activeWeapon = machineGunWeapon;

		}else if (gunMode == GunMode.GRENADE){             // m240 grenade laucnher
			aud.clip = bSFX;
			machineGunGO.SetActive(true);
			machineGunGO.transform.Find ("handLmg").gameObject.SetActive(false);
			machineGunGO.transform.Find ("handL2mg").gameObject.SetActive(true);

			armTargets[0].localPosition = new Vector3(0.17f,-0.8f,armTargets[0].localPosition.z);
			armTargets[1].localPosition = armTargetPos[1];
			gunMode = GunMode.GRENADE;
			hudGun.switchWeapon(3);
			activeWeapon = grenadeWeapon;

		}else if (gunMode == GunMode.FLAME){         ///flame
			aud.clip = fSFX;

			flameThrowerGO.SetActive(true);
			
			gunMode = GunMode.FLAME;
			hudGun.switchWeapon(4);
			armTargets[0].localPosition = armTargetPos[2];
			armTargets[1].localPosition = armTargetPos[3];
			activeWeapon = flameWeapon;
		}else if(gunMode == GunMode.ROCKET){        //bazoooooooooka
			//aud.clip = rpgSFX;
		
			bazookaGO.SetActive(true);
			rpgGO.SetActive(true);
			punchGO.SetActive(false);
			gunMode = GunMode.ROCKET;
			hudGun.switchWeapon(5);
			armTargets[0].localPosition = armTargetPos[4];
			armTargets[1].localPosition = armTargetPos[5];
			activeWeapon = rpgWeapon;
		}else if(gunMode == GunMode.PUNCH){ //PUNCH
			StopCoroutine("canShootAgain");
			canShoot = true;
			gunMode = GunMode.PUNCH;
			bazookaGO.SetActive(true);
			rpgGO.SetActive(false);
			punchGO.SetActive(true);
			hudGun.switchWeapon(6);
			armTargets[0].localPosition = armTargetPos[4];
			armTargets[1].localPosition = armTargetPos[5];
			activeWeapon = punchWeapon;
		}else if(gunMode == GunMode.SHOTGUN){ //SHOTGUN
			StopCoroutine("canShootAgain");
			canShoot = true;
			gunMode = GunMode.SHOTGUN;
			shotGunGO.SetActive(true);
			bazookaGO.SetActive(false);
			rpgGO.SetActive(false);
			punchGO.SetActive(true);
			hudGun.switchWeapon(7);
			armTargets[0].localPosition = armTargetPos[6];
			armTargets[1].localPosition = armTargetPos[7];
			activeWeapon = shotGunWeapon;
		}else if(gunMode == GunMode.STRAT){
			StopCoroutine("canShootAgain");
			canShoot = true;
			gunMode = GunMode.STRAT;
			stratGO.SetActive(true);
			hudGun.switchWeapon(8);
			//print(armTargetPos[8]);
			armIK[0].target = armTargets[4];
			armIK[1].target = armTargets[5];
			activeWeapon = stratWeapon;
		}

		UpdateAmmoDisp();

		lastGunMode = gunMode;
		if(activeWeapon.clip <=0){
			Reload();
		}
	}

	void Flip ()
	{
		//return;
		facingRight = !facingRight;
		float zz = transform.localScale.y * -1f;
		transform.localScale = new Vector3(transform.localScale.x, zz ,1f);
		
		
		//print ("FLIP  "+playerCtrl.facingRight + "   " + facingRight);
		if(facingRight){
			fArmR_SR.sortingOrder = -2; 
			fArmL_SR.sortingOrder = 2; 
			armL_SR.sortingOrder = 1; 
			armIK[0].target = armTargets[0];
			armIK[1].target = armTargets[1];

			armIK[0].angleLimits[0].min = 24f;
			armIK[0].angleLimits[0].max = 160f;
			armIK[1].angleLimits[0].min = 9f;
			armIK[1].angleLimits[0].max = 185f;

			//if(playerCtrl.facingRight){
				//psychoEye.SetActive(false);
			//}else{
				//psychoEye.SetActive(true);
			//}
		}else{
			fArmR_SR.sortingOrder = 5; 
			fArmL_SR.sortingOrder = -1; 
			armL_SR.sortingOrder  = -2; 
			armIK[0].target = armTargets[1];
			armIK[1].target = armTargets[0];

			armIK[0].angleLimits[0].min = 185f;
			armIK[0].angleLimits[0].max = 308f;
			armIK[1].angleLimits[0].min = 220f;
			armIK[1].angleLimits[0].max = 347f;
			
			//if(playerCtrl.facingRight){
				//psychoEye.SetActive(true);
			//}else{
				//psychoEye.SetActive(false);
			//}
		}

		flipHead();
	}

	public void flipHead(){ 
		//print (playerCtrl.facingRight+"  "+facingRight);
		//head

		float zz;
		zz = head.localScale.y * -1f;
		head.localScale = new Vector3(head.localScale.x, zz ,1f);

			if(facingRight){
				psychoEye.SetActive(false);
			}else{
				psychoEye.SetActive(true);
			}

		//print (gunMode);
		if(gunMode == GunMode.UNARMED){//if unarmed switch back the arm targets and return

			fArmR_SR.sortingOrder = -2; 
			fArmL_SR.sortingOrder = 2; 
			armL_SR.sortingOrder = 1; 

			if(grabEnemyJoint){ //but if grabbing enemy switch back arm targets to grab enemy hand IK and return even faster;
				armIK[0].target = armTargets[6];
				armIK[1].target = armTargets[7];
				armIK[0].angleLimits[0].min = 24f;
				armIK[0].angleLimits[0].max = 160f;
				armIK[1].angleLimits[0].min = 9f;
				armIK[1].angleLimits[0].max = 185f;
				return;
			}

			armIK[0].target = armTargets[2];
			armIK[1].target = armTargets[3];
			armIK[0].angleLimits[0].min = 24f;
			armIK[0].angleLimits[0].max = 160f;
			armIK[1].angleLimits[0].min = 9f;
			armIK[1].angleLimits[0].max = 185f;

			return;	
		}
		if(head.localScale.y ==1f){
			fArmR_SR.sortingOrder = -2; 
			fArmL_SR.sortingOrder = 2; 
			armL_SR.sortingOrder = 1; 
			armIK[0].target = armTargets[0];
			armIK[1].target = armTargets[1];
			
			armIK[0].angleLimits[0].min = 24f;
			armIK[0].angleLimits[0].max = 160f;
			armIK[1].angleLimits[0].min = 9f;
			armIK[1].angleLimits[0].max = 185f;
	
		}else{
			fArmR_SR.sortingOrder = 5; 
			fArmL_SR.sortingOrder = -1; 
			armL_SR.sortingOrder = -2; 
			armIK[0].target = armTargets[1];
			armIK[1].target = armTargets[0];
			
			armIK[0].angleLimits[0].min = 185f;
			armIK[0].angleLimits[0].max = 308f;
			armIK[1].angleLimits[0].min = 220f;
			armIK[1].angleLimits[0].max = 347f;
		
		}
		if(gunMode == GunMode.STRAT)
		{
			armIK[0].target = armTargets[4];
			armIK[1].target = armTargets[5];
		}
	}

	void makeBulletShell()
	{

		GameObject shell = (GameObject)Instantiate(activeWeapon.bulletShell, bulletShell_ShotPos.position, Quaternion.identity);
		//pos = Quaternion.Euler(0, 0, 140) * pos;
		float rnd = Random.Range(-.3f,.3f);

		Vector2 angle = Vector2.up;// - transform.root.position).normalized;
		shell.GetComponent<Rigidbody2D>().AddForce (new Vector2(angle.x+ rnd ,angle.y +rnd) * 180f);//new Vector2(pos.x *200f,pos.y*250f));//new Vector2 (-100f, 150f));
		shell.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1f,-20f));
		Destroy (shell, 3f);
	}
	IEnumerator canShootAgain(float delay)
	{
		yield return new WaitForSeconds(delay);
		//print ("canshoot");
		canShoot = true;
		//if(gunMode==GunMode.ROCKET){
		//	rpgGO.SetActive(true);
		//}else if(gunMode==GunMode.PUNCH){
		//	punchGO.SetActive(true);
		//}
	}

	IEnumerator flareLight()
	{
		// Play the fuse audioclip.
		//AudioSource.PlayClipAtPoint(fuse, transform.position);
		
		// Wait for 2 seconds.
		yield return new WaitForSeconds(0.25f);
		gunFlareLight.enabled = false;
		//gunFlareLight2.enabled = false;
		// Explode the bomb.
		//Explode();
	}
	void flameFXOn()
	{
		flameLight.intensity += .5f;
		flameAud.volume += .03f;
		if(flameLight.intensity > 5){
			CancelInvoke();
		}
	}
	void flameFXOff()
	{
		flameLight.intensity -= 1f;
		flameAud.volume -= .05f;
		if(flameLight.intensity < 0.3f){
			CancelInvoke();
			flameLight.enabled = false;
			flameAud.enabled = false;
			flameAud.volume = 0f;
		}
	}
	IEnumerator handGunflareLight()
	{
		// Play the fuse audioclip.
		//AudioSource.PlayClipAtPoint(fuse, transform.position);
		
		// Wait for 2 seconds.
		yield return new WaitForSeconds(0.25f);
		handGunFlareLight.enabled = false;
		//handGunFlareLight2.enabled = false;
		// Explode the bomb.
		//Explode();
	}
	IEnumerator shotGunflareLight()
	{
		// Play the fuse audioclip.
		//AudioSource.PlayClipAtPoint(fuse, transform.position);
		
		yield return new WaitForSeconds(0.1f);
		shotGunFlare.Stop();
		yield return new WaitForSeconds(0.15f);
		shotGunFlareLight.enabled = false;
		//shotGunFlareLight2.enabled = false;
		yield return new WaitForSeconds(0.15f);
		makeBulletShell();
		// Explode the bomb.
		//Explode();
	}
	public void fixGuns()
	{
		aud.Stop ();
		canShoot = true;
		if(gunMode==GunMode.ROCKET){
			rpgGO.SetActive(true);
		}else if(gunMode==GunMode.PUNCH){
			punchGO.SetActive(true);
		}
		flame.Stop ();
		smoke.Stop ();
		flameLight.enabled = false;
		flameAud.enabled = false;
		flameAud.volume = 0f;
		headSR.sprite = headS[0];
		kickHit.gameObject.SetActive(false);
		canKick = true;
		if(grabEnemyJoint){
			unGrab();
			ReleaseEnemy();
		}

	}
	//Score code spawns pickup
	public void SpawnPickup(Vector3 pos)
	{
		GameObject pu = (GameObject)Instantiate(pickupCrate, pos, transform.rotation);
		//GunMode gm = GunMode.UNARMED;
		int count = 0;
		float low = 110f;//lowest percentage
		int lowNum = 0;//which num in array
		foreach (Weapon w in weaponsForAmmo) {//get percentage of how full of ammo he is for all guns he has
			if(w.hasGun){
				float diff = w.ammo/w.maxAmmo * 100;//((float)w.clip + w.ammo)/w.maxAmmo * 100;
				//print (diff);

				if(diff<low){//Get the lowest
					low = diff;
					lowNum = count;
				}
			}
			count++;
		}

		var puCS = pu.GetComponent<PickupTrigger>();
		//if full of ammo 
		if(low >= 100){
			if(playerHealth.health==100){//if full of health
				int i = UnityEngine.Random.Range(0,5)+2;
				//print ("random "+i);
				puCS.gunMode = gunModes[i];//make it random amo
				puCS.howMuch = weapons[i].pickupAmmo;

			}else{
				puCS.pickupMode = global::PickupTrigger.PickupMode.HEALTH;
			}
		}else{ //not full of ammo
			if(playerHealth.health<low){
				puCS.pickupMode = global::PickupTrigger.PickupMode.HEALTH;
			}else{
				puCS.gunMode = gunModes[lowNum+2];
				puCS.howMuch = weapons[lowNum+2].pickupAmmo;
			}

		}
		//print ("LOW "+lowNum+"  "+low);
	
	}
	public void PickUpAmmoCrate()
	{
		if(activeWeapon.ammo<activeWeapon.maxAmmo){
			Pickup(global::PickupTrigger.PickupMode.AMMO,gunMode,activeWeapon.pickupAmmo);
			return;
		}
		//GunMode gm = GunMode.UNARMED;
		int count = 0;
		float low = 110f;//lowest percentage
		int lowNum = 0;//which num in array
		foreach (Weapon w in weaponsForAmmo) {//get percentage of how full of ammo he is for all guns he has
			if(w.hasGun){
				float diff = w.ammo/w.maxAmmo * 100;
				//print (diff);
				
				if(diff<low){//Get the lowest
					low = diff;
					lowNum = count;
				}
			}
			count++;
		}
		
		//Pickup puCS = pu.GetComponent<Pickup>();
		//if  not full of ammo 
		if(low < 100){
			
				//puCS.gunMode = gunModes[lowNum+2];
				//puCS.howMuch = weapons[lowNum+2].pickupAmmo;
			print (gunModes[lowNum+2]);
			Pickup(global::PickupTrigger.PickupMode.AMMO,gunModes[lowNum+2],weapons[lowNum+2].pickupAmmo);
		}
		//print ("LOW "+lowNum+"  "+low);
		
	}
	//pickup guns or ammo
	public void Pickup(global::PickupTrigger.PickupMode pickupMode, GunMode gMode, int howMuch)
	{
		//print (pickup);
		AudioSource.PlayClipAtPoint(reloadFX[0], transform.position);
		if(pickupMode == global::PickupTrigger.PickupMode.WEAPON){
			GetComponent<Gun>().SwitchToWeapon(gMode);
			//AudioSource.PlayClipAtPoint(pickupFX[0], transform.position);
			switch(gMode){
			case Gun.GunMode.HANDGUN:
				handGunWeapon.hasGun = true;
				weaponWheel.transform.Find("1").GetComponent<Toggle>().interactable = true;
				break;
			case Gun.GunMode.MACHINEGUN:
				machineGunWeapon.hasGun = true;
				weaponWheel.transform.Find("2").GetComponent<Toggle>().interactable = true;

				break;
			case Gun.GunMode.GRENADE:
				grenadeWeapon.hasGun = true;
				weaponWheel.transform.Find("3").GetComponent<Toggle>().interactable = true;
				transform.Find ("weapon Transform").Find ("machineGun3D").Find("grenadeLauncher").gameObject.SetActive(true);
				transform.Find ("weapon Transform").Find ("machineGun3D").Find("handRmg").localPosition = new Vector3(1.1f,-0.25f,-0.2f);//move hand down for launcher//had to make separate hands once I animated for reload they wouldnt move

				break;
			case Gun.GunMode.FLAME:
				flameWeapon.hasGun = true;
				weaponWheel.transform.Find("4").GetComponent<Toggle>().interactable = true;
				break;
			case Gun.GunMode.ROCKET:
				rpgWeapon.hasGun = true;
				weaponWheel.transform.Find("5").GetComponent<Toggle>().interactable = true;

				break;
			case Gun.GunMode.PUNCH:
				punchWeapon.hasGun = true;
				weaponWheel.transform.Find("6").GetComponent<Toggle>().interactable = true;


				break;
			case Gun.GunMode.SHOTGUN:
				shotGunWeapon.hasGun = true;
				weaponWheel.transform.Find("7").GetComponent<Toggle>().interactable = true;
				
				
				break;
			case Gun.GunMode.STRAT:
				stratWeapon.hasGun = true;
				weaponWheel.transform.Find("8").GetComponent<Toggle>().interactable = true;
				
				
				break;
			}
			
			
		}else{//ammo
			Weapon whichAmmo= handGunWeapon;//new Weapon(gunMode,transform);
			switch(gMode){

				case Gun.GunMode.MACHINEGUN:
					whichAmmo = machineGunWeapon;
				break;
				case Gun.GunMode.GRENADE:
					whichAmmo = grenadeWeapon;
				break;
				case Gun.GunMode.FLAME:
					whichAmmo = flameWeapon;
				break;
				case Gun.GunMode.ROCKET:
					whichAmmo = rpgWeapon;
				break;
				case Gun.GunMode.PUNCH:
					whichAmmo = punchWeapon;
				break;
				case Gun.GunMode.SHOTGUN:
				whichAmmo = shotGunWeapon;
				break;
			}
			whichAmmo.ammo += howMuch;
			if(whichAmmo.ammo > whichAmmo.maxAmmo){
				whichAmmo.ammo = whichAmmo.maxAmmo;
			}
			UpdateAmmoDisp();
		}
	}
	//public void SetCameraMaxMin(float[] maxMin)
	//{
	//	cameraXmaxMin = maxMin;
	//}
}

public class Weapon

{
	public bool hasGun = false;
	public Gun.GunMode gunType;
	public int clip = 8; //rounds in the clip
	public int maxClip;  //max rounds the clip can hold
	public int ammo;  //ammo for gun
	public int maxAmmo; //maximum ammo you can carry for gun
	public float reloadTime;// time it takes to reload
	public int chargeCount = 0; // for laser stratocaster charge

	public Transform shotPos;// where projectile should originate from
	public GameObject bulletShell;// bullet shell/casing ref;
	public int pickupAmmo;//how much is in a crate of ammo for this gun?
	public Weapon(Gun.GunMode gm,Transform weaponT)
	{
		switch(gm)
		{
		case Gun.GunMode.UNARMED:
			
			clip = 0;//maxClip = 8;
			ammo = 0;//int.MaxValue;
			maxAmmo = 0;//int.MaxValue;
			reloadTime = 0;// 1.2f;
			shotPos = weaponT;//weaponT.Find ("handGun3D").Find ("handGun_spot");
			hasGun = true;
			//bulletShell = bulletShells [0];
			break;
		case Gun.GunMode.HANDGUN:

			clip = maxClip = 8;
			ammo = int.MaxValue;
			maxAmmo = int.MaxValue;
			reloadTime = 1.2f;
			shotPos = weaponT.Find ("handGun3D").Find ("handGun_spot");
			//bulletShell = bulletShells [0];
			break;
		case Gun.GunMode.MACHINEGUN:
			clip = maxClip = 20;
			ammo = 60;
			maxAmmo = 120;
			reloadTime = 1.3f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("machineGun3D").Find ("machineGun_spot");
			//bulletShell = bulletShells [0];
			break;
		case Gun.GunMode.GRENADE:
			clip = maxClip = 1;
			ammo = 15;
			maxAmmo = 30;
			reloadTime = 1f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("machineGun3D").Find ("grenadeLauncher_spot");
			break;
		case Gun.GunMode.FLAME:
			clip = maxClip = 60;
			ammo = 120;
			maxAmmo = 240;
			reloadTime = 2f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("machineGun3D").Find ("machineGun_spot");
			break;
		case Gun.GunMode.ROCKET:
			clip = maxClip = 1;
			ammo = 10;
			maxAmmo = 20;
			reloadTime = 1.3f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("bazooka3D").Find ("bazooka_spot");
			break;
		case Gun.GunMode.PUNCH:
			clip = maxClip = 1;
			ammo = 6;
			maxAmmo = 12;
			reloadTime = 1.3f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("bazooka3D").Find ("bazooka_spot");
			break;
		case Gun.GunMode.SHOTGUN:
			clip = maxClip = 4;
			ammo = 12;
			maxAmmo = 24;
			reloadTime = 2f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("shotGun3D").Find ("shotGun_spot");
			//bulletShell = bulletShells [1];
			break;
		
		case Gun.GunMode.STRAT:
			clip = maxClip = int.MaxValue;
			ammo = int.MaxValue;
			maxAmmo = int.MaxValue;
			reloadTime = 2f;
			pickupAmmo = ammo;
			shotPos = weaponT.Find ("stratGun3D").Find ("stratGun_spot");
			//bulletShell = bulletShells [1];
			break;
		}
	}
}