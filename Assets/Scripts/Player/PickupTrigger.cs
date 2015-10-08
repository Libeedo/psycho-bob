using UnityEngine;
using System.Collections;

public class PickupTrigger : Pickup{

	//has to be  WEAPON pickup MODE for trigger
	void OnTriggerEnter2D (Collider2D col) 
	{
		//print ("hey "+pickupMode);
		
		if(col.tag == "Player"){
			//if(pickupMode == PickupMode.HEALTH){
				//col.GetComponent<PlayerHealth>().AddHealth(howMuch);
			//}else if(pickupMode == PickupMode.COIN){
				//Level.instance.CollectCoin();
			//}else{
				col.GetComponent<PlayerControl>().gunCS.Pickup(pickupMode,gunMode,howMuch);

			//}
			Destroy(gameObject);
		}else if(col.tag == "Nurse"){
			//if(pickupMode == PickupMode.HEALTH){
				//Level.instance.GetPlayerTransform().GetComponent<PlayerHealth>().AddHealth(howMuch);
				
				
			//}else{
				Level.instance.GetPlayerTransform().GetComponent<PlayerControl>().gunCS.Pickup(pickupMode,gunMode,howMuch);
				
			//}
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
