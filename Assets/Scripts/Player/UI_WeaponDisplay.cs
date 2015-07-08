using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_WeaponDisplay : MonoBehaviour
{
	public Sprite[] textures;

	//public GameObject shadow;
	public void switchWeapon(int weapon)
	{

			transform.GetComponent<Image>().sprite= textures[weapon];
			//shadow.GetComponent<GUITexture>().texture = textures[weapon];
		
	}

}
