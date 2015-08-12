using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FadeColour : MonoBehaviour {
	private Color clr;
	private List<SpriteRenderer> spriteRens =  new List<SpriteRenderer>();
	// Use this for initialization
	void OnEnable () {
		clr = new Color(.2f,.2f,.3f,1f);
		//int count = 0;
		var spriteRens2 = GetComponentsInChildren<SpriteRenderer>();
		spriteRens = spriteRens2.ToList();
		InvokeRepeating("DoIt",0,0.015f);
		StartCoroutine("StopIt");
	}
	
	// Update is called once per frame
	void DoIt () {

		foreach (SpriteRenderer sr in spriteRens){
			sr.material.color = Color.Lerp(sr.material.color,clr,Time.deltaTime);
		
		}

	}
	IEnumerator StopIt()
	{
		yield return new WaitForSeconds(2);
		CancelInvoke();
	}
	public void RemoveSprite(SpriteRenderer s)
	{
		spriteRens.Remove(s);
	}

}
