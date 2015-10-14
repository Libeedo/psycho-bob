﻿using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public static MainMenu instance;
	public AudioClip buttonFX;
	public string[] levels;
	private bool[] levelsComplete = new bool[10];
	private GameObject[] levelButtons = new GameObject[10];
	private int selectedLevel = 0;
	// Use this for initialization
	void Start () {
		if(instance){
			Destroy (gameObject);
		}else{
			instance = this;
		}
		DontDestroyOnLoad(transform.gameObject);

	}

	public void SwitchLevel (int which) {
		selectedLevel = which;
		AudioSource.PlayClipAtPoint(buttonFX,Vector2.zero);
		//Application.LoadLevel ("ThunderDome");
	}
	public void PlayLevel () {

		Application.LoadLevel (levels[selectedLevel]);//"ThunderDome");
	}
	public void CompleteLevel()
	{
		levelsComplete[selectedLevel] = true;
		selectedLevel++;
	}
}
