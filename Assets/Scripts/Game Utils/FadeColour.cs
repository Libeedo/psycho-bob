using UnityEngine;
using System.Collections;

public class FadeColour : MonoBehaviour {
	private Color clr;
	private SpriteRenderer[] spriteRens;
	// Use this for initialization
	void Start () {
		clr = new Color(.2f,.2f,.3f,1f);
		//int count = 0;
		spriteRens = GetComponentsInChildren<SpriteRenderer>();
		Destroy (this,2);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		foreach (SpriteRenderer sr in spriteRens){
			sr.material.color = Color.Lerp(sr.material.color,clr,Time.deltaTime);
		
		}

	}
}
