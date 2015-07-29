using UnityEngine;
using System.Collections;

public class SniperSight : MonoBehaviour {
	private int shootLayerMsk; 
	private Transform frontCheck;
	//private bool canShoot = true;
	private Oscillate oscCS;
	private Animator enemyA;
	public GameObject bulletRef;
	public AudioClip[] shootFX;
	private RaycastHit2D hit;
	// Use this for initialization
	public bool soloMode = false; //soloMode = not attached to soldier (ie dickbat or drone) //needed still? dont have shooting dickbats really anymore

	private Animator headA;
	void Awake()
	{
		oscCS = GetComponent<Oscillate>();
		if (soloMode) {
			enemyA = transform.GetComponent<Animator> ();
		} else {
				enemyA = transform.root.GetComponent<Animator> ();
				headA = enemyA.transform.Find ("head Transform").Find ("head").GetComponent<Animator> ();
		}
	}
	void Start () {
		int layerMsk2 = 1 << LayerMask.NameToLayer("Props");
		int layerMsk1 = 1 << LayerMask.NameToLayer("Ground");
		int layerMsk3 = 1 << LayerMask.NameToLayer("Player");
		int layerMsk4 = 1 << LayerMask.NameToLayer("Enemies");
		//int layerMsk5 = 1 << LayerMask.NameToLayer("Pickups");
		shootLayerMsk = layerMsk1 | layerMsk2 |  layerMsk3 | layerMsk4;
		frontCheck = transform.Find ("frontCheck");

	
		//InvokeRepeating("TestShoot",0,.3f);
	}
	
	// Update is called once per frame

	public void TestShoot()//enemy projectile
	{
		//frontCheck.position, frontCheck.position + (frontCheck.right*-40f)
		Vector3 pos = (frontCheck.position-transform.position).normalized;
		Debug.DrawLine(frontCheck.position,frontCheck.position+(pos*50f), Color.red);
		if(hit = Physics2D.Linecast (frontCheck.position,frontCheck.position+(pos*50f) , shootLayerMsk))
		{
			//if(Random.Range(0,5)>2){
				//print (hit.transform.tag);
				if(hit.transform.tag == "Player"){
					StartCoroutine("Shoot");
				}
			//}
			
		}
	}
	public IEnumerator Shoot()
	{
		//StopSniping();
		CancelInvoke();
		oscCS.enabled = false;

		enemyA.Play("enemyShootGun");
		//if (headSR) {
				//headSR.sprite = Soldier_Sprites.S.getHead (3);
		//}
		headA.Play ("enemyShootGunMouth");
		AudioSource.PlayClipAtPoint(shootFX[shootFX.Length-1], transform.position);
		yield return new WaitForSeconds(0.6f);
		AudioSource.PlayClipAtPoint(shootFX[Random.Range (0, shootFX.Length-1)], transform.position);
		//Vector3  playerPos = hit.transform.position;
		//playerPos.y += 2;
		Vector3 pos = new Vector3(transform.position.x + (transform.root.transform.localScale.x *3f),transform.position.y);//-2f,0);
		//float angle2 = Mathf.Atan2(playerPos.y - pos.y, playerPos.x - pos.x) * Mathf.Rad2Deg;
		//Quaternion angle3 = Quaternion.Euler(new Vector3(0, 0, angle2));

		Instantiate(bulletRef, pos, Quaternion.identity);//angle3);
		//b.GetComponent<Animator>().
		//go.rigidbody2D.AddForce(new Vector2(3000f * transform.localScale.x,0f));
		
		yield return new WaitForSeconds(0.3f);
		

		enemyA.Play ("enemySnipe");
		//if (headSR) {
			//headSR.sprite = Soldier_Sprites.S.getHead (0);
		//}
		yield return new WaitForSeconds(1f);
		StartSniping();
	}
	public void StartSniping()
	{
		CancelInvoke(); 
		InvokeRepeating("TestShoot",0,.3f);
		oscCS.enabled = true;
		//print ("start snipe");
	}
	public void StopSniping()
	{
		//print("Stop snipe");
		CancelInvoke();
		StopCoroutine("Shoot");
		oscCS.enabled = false;
	}
}
