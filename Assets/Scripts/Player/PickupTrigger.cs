using UnityEngine;
using System.Collections;

public class PickupTrigger : MonoBehaviour{

	public enum PickupMode
	{
		AMMO,
		WEAPON,
		HEALTH
		
	}
	public PickupMode pickupMode = PickupMode.WEAPON;
	public Gun.GunMode gunMode = Gun.GunMode.HANDGUN;
	public int howMuch = 5;
	public Sprite[] symbols;
	//private Light lightt;
	//private float lightIntensity = 1.5f;
	void Start()
	{
		//lightt = transform.Find ("light").GetComponent<Light>();

		//lightt.intensity = 0.3f;

		if(pickupMode == PickupMode.AMMO){
			SpriteRenderer sr = transform.Find("symbol").GetComponent<SpriteRenderer>();
			if(gunMode == Gun.GunMode.MACHINEGUN){
				sr.sprite = symbols[0];
			}else if(gunMode == Gun.GunMode.GRENADE){
				sr.sprite = symbols[1];
			}else if(gunMode == Gun.GunMode.FLAME){
				sr.sprite = symbols[2];
			}else if(gunMode == Gun.GunMode.ROCKET){
				sr.sprite = symbols[3];
			}else if(gunMode == Gun.GunMode.PUNCH){
				sr.sprite = symbols[4];
			}else if(gunMode == Gun.GunMode.SHOTGUN){
				sr.sprite = symbols[5];
			}
		}else if(pickupMode == PickupMode.HEALTH){
			transform.Find("symbol").GetComponent<SpriteRenderer>().sprite = symbols[6];
			howMuch = 40;
		}
		//InvokeRepeating("PulseLight",0,.1f);
	}
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print ("hey "+pickupMode);
		
		if(col.tag == "Player"){
			if(pickupMode == PickupMode.HEALTH){
				col.GetComponent<PlayerHealth>().AddHealth(howMuch);


			}else{
				col.GetComponent<PlayerControl>().gunCS.Pickup(pickupMode,gunMode,howMuch);

			}
			Destroy(gameObject);
		}else if(col.tag == "Nurse"){
			if(pickupMode == PickupMode.HEALTH){
				Level.instance.GetPlayerTransform().GetComponent<PlayerHealth>().AddHealth(howMuch);
				
				
			}else{
				Level.instance.GetPlayerTransform().GetComponent<PlayerControl>().gunCS.Pickup(pickupMode,gunMode,howMuch);
				
			}
			Destroy(gameObject);
		}
	}
	/*void PulseLight()
	{
		lightt.intensity = Mathf.Lerp(lightt.intensity, lightIntensity, 15f * Time.deltaTime);
		//light.intensity += lightIntensity;

		if(lightt.intensity > 1.1f ){
			lightIntensity = 0.01f;
		}else if(lightt.intensity < 0.5f){
			lightIntensity = 1.5f;
		}
	}*/
}
