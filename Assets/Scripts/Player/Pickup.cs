using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	public enum PickupMode
	{
		AMMO,
		WEAPON,
		HEALTH,
		COIN
		
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
}
