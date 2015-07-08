using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour {
	private bool paused = false;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.P))
		{
			paused = !paused;
			if(paused){
				Pause ();
			}else{
				Unpause();
			}
		}


	}
	public void Pause()
	{
		Time.timeScale = 0;
		if(GameObject.FindGameObjectWithTag("Player")){
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().gunCS.enabled = false;
		}
		Cursor.visible = true;
	}
	public void Unpause()
	{
		Time.timeScale = 1;
		if(GameObject.FindGameObjectWithTag("Player")){
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().gunCS.enabled = true;
		}
		Cursor.visible = false;
	}
}
