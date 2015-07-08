using UnityEngine;
using System.Collections;

public class HitColour : MonoBehaviour
{


//	private int hitCount = 0;

	//private SpriteRenderer spRen;
	public GameObject lightObject;
	private Light lighte;
	void Awake()
	{
		lighte = lightObject.transform.GetComponent<Light>();
	
	}
	
	public void Hurt(float delay)
	{
		//print("hit clr: "+gameObject);
		lighte.enabled = true;
		//spRen = sp;
		//spRen.color = new Color(2f, 0.5f, 0f, 1f);
		StartCoroutine(ColourBack(delay));

	
	}
	





	IEnumerator ColourBack(float d)
	{


		// Wait for 2 seconds.
		yield return new WaitForSeconds(d);
		//spRen.color =  new Color(1f, 1f, 1f, 1f);
		lighte.enabled = false;
	}
}
