using UnityEngine;
using System.Collections;

public class Boss_AssTank : Boss {

	public CombatZone combatZone;

	private Animator drqAnim;
	private Animator anim;

	private bool walking = false;
	private float speed = -0.03f;
	private float waitTime = 2f;
	private AudioSource footAud;
	private int lives = 3;//lives are the different stages/phases the boss goes thru
	void Start()
	{
		drqAnim = transform.Find("assTank_body").Find("assTank_pod").Find("drQ").GetComponent<Animator>();
		anim = GetComponent<Animator>();
		footAud = GetComponent<AudioSource>();
		//InvokeRepeating("SwitchWalking",5f,1f);
		StartCoroutine("SwitchWalking");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(walking){
			var p = transform.position;
			p.x += speed;
			transform.position = p;
		}
	}

	IEnumerator SwitchWalking()
	{
		yield return new WaitForSeconds(waitTime);
		if(walking){
			walking = false;
			anim.Play("assTank_IDLE");
			footAud.enabled = false;
		}else{
			var r = Random.Range(0,1f);
			print (r);
			if(r> 0.7f){
				walking = true;
				footAud.enabled = true;
				anim.Play("assTank_WALK");
				waitTime = Random.Range(1f,4f);
				if(transform.position.x > Level.instance.GetPlayerX()){
					if(speed>0){
						speed = -speed;
					}
				}else{
					if(speed<0){
						speed = -speed;
					}
				}
			}

		}
		StartCoroutine("SwitchWalking");
	}


	public override void Hurt(float dmg, bool explosion)
	{
		base.Hurt (dmg,explosion);
		drqAnim.SetTrigger ("hurt");
		if(dead){
			combatZone.EndZone();
		}
	}


}
