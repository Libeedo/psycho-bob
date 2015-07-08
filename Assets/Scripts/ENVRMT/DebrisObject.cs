using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebrisObject : MonoBehaviour
{

	public int HP = 5;					// How many times the enemy can be hit before it dies.

	public bool glassMode = false;
	public GameObject switchModel1;
	public GameObject switchModel2;
	public AudioClip SFX;

	public bool cockPitGlass = false;
	private Vehicle vehicleCS;
	private List<DebrisPiece> pieces;
	void Start()
	{
		pieces = new List<DebrisPiece>();
		if(cockPitGlass)
		{

			vehicleCS = transform.root.transform.GetComponent<Vehicle>();
		}
		foreach (Transform child in transform) {
			pieces.Add(child.GetComponent<DebrisPiece>());
			if(glassMode){

				child.GetComponent<DebrisPiece>().glassMode = true;
			}

		}
	}


	public void Death()
	{
			foreach (DebrisPiece dp in pieces){//Transform child in transform) {
			//print ("DO "+child.name);
				//child.GetComponent<DebrisPiece>().Death ();
				dp.Death();
			}
		if(glassMode){
			switchModel1.SetActive(false);//switch models
			switchModel2.SetActive(true);
			AudioSource.PlayClipAtPoint(SFX, transform.position);
		}
		if(cockPitGlass)
		{
			vehicleCS.cockPitBroken();
		}
	}





}
