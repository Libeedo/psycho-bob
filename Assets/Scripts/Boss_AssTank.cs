using UnityEngine;
using System.Collections;

public class Boss_AssTank : Boss {

	public CombatZone combatZone;

	private ParticleSystem drqBlood;
	private Animator drqAnim;
	private Animator anim;

	private bool walking = false;
	private float speed = -0.03f;
	private float waitTime = 2f;
	private AudioSource footAud;
	private int lives = 2;//lives are the different stages/phases the boss goes thru
	private delegate void HurtState();
	private HurtState hurtState;
	void Start()
	{
		drqAnim = transform.Find("assTank_body").Find("assTank_pod").Find("drQ").GetComponent<Animator>();
		drqBlood = drqAnim.transform.Find ("blood").GetComponent<ParticleSystem>();
		anim = GetComponent<Animator>();
		footAud = GetComponent<AudioSource>();
		//InvokeRepeating("SwitchWalking",5f,1f);

		hurtState = Hurt1;

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
		hurtState();
		if(dead){
			lives--;
			var p = transform.Find("assTank_body").Find("assTank_pod");
			if(lives== 0){
				combatZone.EndZone();
				p.Find ("assTank_turret").gameObject.SetActive(false);
				p.Find ("assTank_turret animate").gameObject.SetActive(true);
				anim.Play ("assTank_DEATH");
				this.enabled = false;
			}else if(lives == 1){
				HP = 20;
				dead = false;

				p.Find ("glassShatter").GetComponent<TriggerDebris>().Break();
				p.Find ("assTank_turret").Find ("assTank_glass3D").gameObject.SetActive(false);
				p.Find ("assTank_turret").Find ("assTank_glassBroken").gameObject.SetActive(true);
				hurtState = Hurt2;
			}
		}
	}
	private void Hurt1()
	{
		drqAnim.SetTrigger ("hurt");
		anim.Play ("assTank_HURT");
	}
	private void Hurt2()
	{
		drqAnim.SetTrigger ("hurt");
		drqBlood.Emit(300);
	}
}
