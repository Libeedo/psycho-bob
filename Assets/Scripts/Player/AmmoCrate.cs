using UnityEngine;
using System.Collections;

public class AmmoCrate : MonoBehaviour {



	private bool canGiveAmmo = true;

	void OnTriggerStay2D (Collider2D col) 
	{
		//print ("hey "+col.name);
		if(canGiveAmmo){
			if(col.tag == "Player"){
				StartCoroutine("CantGiveAmmo");
				col.GetComponent<PlayerControl>().gunCS.PickUpAmmoCrate();
					

				
			}else if(col.tag == "Nurse"){
				StartCoroutine("CantGiveAmmo");
				Level.instance.GetPlayerTransform().GetComponent<PlayerControl>().gunCS.PickUpAmmoCrate();
					
				
			
			}
		}
	}
	IEnumerator CantGiveAmmo()
	{
		canGiveAmmo = false;
		yield return new WaitForSeconds(1.3f);
		canGiveAmmo = true;
	}

}
