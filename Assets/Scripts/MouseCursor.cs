using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour {
	private Transform xhairHUD;
	void Start()
	{
		xhairHUD =GameObject.FindGameObjectWithTag("HUD").transform.Find ("xhairHUD");
	}
	
	// Update is called once per frame
	void Update () {
		xhairHUD.position = Input.mousePosition;
	}
}
