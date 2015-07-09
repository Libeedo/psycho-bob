using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public enum PickupMode
	{
		AMMO,
		WEAPON,
		HEALTH
		
	}
	public PickupMode pickupMode = PickupMode.WEAPON;
	public Gun.GunMode gunMode = Gun.GunMode.HANDGUN;
	public int howMuch = 5;
}
