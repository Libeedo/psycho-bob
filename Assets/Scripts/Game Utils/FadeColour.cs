using UnityEngine;
using System.Collections;

public class FadeColour : MonoBehaviour {
	private Color clr;
	private SpriteRenderer[] spriteRens;
	// Use this for initialization
	void OnEnable () {
		clr = new Color(.2f,.2f,.3f,1f);
		//int count = 0;
		spriteRens = GetComponentsInChildren<SpriteRenderer>();
		InvokeRepeating("DoIt",0,0.03f);
		Destroy (this,2);
	}
	
	// Update is called once per frame
	void DoIt () {

		foreach (SpriteRenderer sr in spriteRens){
			sr.material.color = Color.Lerp(sr.material.color,clr,Time.deltaTime);
		
		}

	}
}
