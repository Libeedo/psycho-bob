using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{	
	public float health = 100f;					// The player's health.
	public float repeatDamagePeriod = 4f;		// How frequently the player can be damaged.
	public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	private float hurtForce = 700f;				// The force with which the player is pushed when hurt.
	public float damageAmount = 20f;			// The amount of damage to take when enemies touch the player

	private Image healthBar;
	private bool canBeHit = true;
	//private float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).
	private PlayerControl playerControl;		// Reference to the PlayerControl script.
	private Animator anim;						// Reference to the Animator on the player

	public AudioClip healthFX;
	//public Sprite headSpr;

	private int pLayer; //player layer
	private int eLayer;//enemy layer

	void Awake ()
	{
		// Setting up references.
		playerControl = GetComponent<PlayerControl>();
		healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
		anim = GetComponent<Animator>();

		// Getting the intial scale of the healthbar (whilst the player has full health).
		healthScale = healthBar.transform.localScale;

		pLayer = LayerMask.NameToLayer ("Player");
		eLayer = LayerMask.NameToLayer ("Enemies");
	}


	void OnCollisionEnter2D (Collision2D col)
	{

		// If the colliding gameobject is an Enemy...
		if(col.gameObject.tag == "Enemy" || col.gameObject.tag == "EnemyHead")
		{
			if(playerControl.sliding)// && Mathf.Abs(playerControl.gameObject.rigidbody2D.velocity.x) >4f)
			{
				//print("tripped");
				col.transform.root.GetComponent<Enemy>().Tripped(playerControl.facingRight, playerControl.gameObject.GetComponent<Rigidbody2D>().velocity);
				return;
				
			}
			Hurt (false,col.transform);
			col.transform.root.GetComponent<Enemy>().HurtPlayer(transform.position); 

		
		}else if(col.gameObject.tag == "HurtPlayer" || col.gameObject.tag == "Fireball")// || col.gameObject.tag == "EnemyFetus")
		{
			Hurt (false,col.transform);

		}else if(col.gameObject.tag == "Pickup")
		{
			var m = col.gameObject.GetComponent<Pickup>().pickupMode;
			if(m == Pickup.PickupMode.HEALTH){
				health += col.gameObject.GetComponent<Pickup>().howMuch;
				if(health > 100f){
					health = 100f;
				}
				UpdateHealthBar();
			}else if(m == Pickup.PickupMode.AMMO){
				var pt = col.gameObject.GetComponent<Pickup>();

				playerControl.gunCS.Pickup(m,pt.gunMode,pt.howMuch);
				//AudioSource.PlayClipAtPoint(pickupFX[0], transform.position);
			}else if(m == Pickup.PickupMode.COIN){
				Level.instance.CollectCoin();
			}
			Destroy (col.gameObject);
		}
	}
	public void AddHealth(float add)
	{
		health += add;
		if(health > 100f){
			health = 100f;
		}
		AudioSource.PlayClipAtPoint(healthFX, transform.position);
		UpdateHealthBar();
	}
	public void Hurt(bool explosion, Transform enemy)
	{
		// ... and if the time exceeds the time of the last hit plus the time between hits...
		if (canBeHit) 
		{
			// ... and if the player still has health...
			//if(health > 0f)
			//{
			if(explosion){
				var dis = Vector2.Distance(transform.position,enemy.position) * 8;
				damageAmount = 100 - dis;
			}	
			TakeDamage(enemy); 
			StartCoroutine("CanBeHit");
				//lastHitTime = Time.time; 
			//}
			// If the player doesn't have health, do some stuff, let him fall into the river to reload the level.
			if(health < 0f)
			{
				Die ();
			}
		}

	}

	void TakeDamage (Transform enemy)
	{
		
		
		// Make sure the player can't jump.
		playerControl.jump = false;
		
		// Create a vector that's from the enemy to the player with an upwards boost.
		Vector3 hurtVector = (transform.position - enemy.position + Vector3.up).normalized;
		
		// Add a force to the player in the direction of the vector and multiply by the hurtForce.
		GetComponent<Rigidbody2D>().AddForce(hurtVector * hurtForce);
		
		
		health -= damageAmount;
		
		// Update what the health bar looks like.
		UpdateHealthBar();
		anim.SetTrigger ("Hurt");
		
		Level.instance.makePlayerHitNum (transform.position, damageAmount/10);
		
		AudioSource.PlayClipAtPoint(ouchClips[Random.Range (0, ouchClips.Length)], transform.position);
	}

	private IEnumerator CanBeHit()
	{
		canBeHit = false;
		Physics2D.IgnoreLayerCollision(pLayer,eLayer,true);
		yield return new WaitForSeconds (repeatDamagePeriod);
		canBeHit = true;
		damageAmount = 20;
		Physics2D.IgnoreLayerCollision(pLayer,eLayer,false);
	}
	public void Die()
	{
		StopCoroutine ("CanBeHit");
		canBeHit = false;
		damageAmount = 20;
		GetComponent<Collider2D>().isTrigger = true;
		//rigidbody2D.isKinematic = true;
		if(!playerControl.facingRight){
			playerControl.Flip();
		}
		//anim.Play("Death");
		//anim.SetTrigger("Die");
		playerControl.SaddleDown ();
		playerControl.enabled = false;
		Level.instance.KillPlayer();
		canBeHit = true;
		// Find all of the colliders on the gameobject and set them all to be triggers.
		/*Collider2D[] cols = GetComponents<Collider2D>();
				foreach(Collider2D c in cols)
				{
					c.isTrigger = true;
				}
				
				// Move all sprite parts of the player to the front
				SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
				foreach(SpriteRenderer s in spr)
				{
					s.sortingLayerName = "UI";
				}
				
				// ... disable user Player Control script
				GetComponent<PlayerControl>().enabled = false;
				
				// ... disable the Gun script to stop a dead guy shooting a nonexistant bazooka
				GetComponentInChildren<Gun>().enabled = false;
				
				// ... Trigger the 'Die' animation state
				*/
	}

	public void Spawn()
	{
		//print("player spawn");
		playerControl.enabled = true;

		GetComponent<Collider2D>().isTrigger = false;
		//rigidbody2D.isKinematic = false;
		GetComponent<Rigidbody2D>().gravityScale = 1f;
		//anim.Play("heroSpawn");
		Physics2D.IgnoreLayerCollision(pLayer,eLayer,false);
		Level.instance.spawnPoint.Spawn ();

		GetComponent<Animator>().enabled = false;
		//transform.Find("weapon").GetComponent<Gun>().fixGuns();
		GetComponent<Animator>().enabled = true;
		//anim.SetBool ("Shooting",false);
		anim.ResetTrigger("Shoot");
		//anim.Play("HeadIdle");//for head
		playerControl.Spawn();

		//transform.Find("head Transform").transform.Find ("head").GetComponent<SpriteRenderer>().sprite = headSpr;
		//lastSpawnPoint.GetComponent<SpawnPoint>().Spawn();
	}

	public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);
		//healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);

		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
	}
}
