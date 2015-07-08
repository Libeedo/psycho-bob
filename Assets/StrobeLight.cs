using UnityEngine;
using System.Collections;

public class StrobeLight : MonoBehaviour {

	private Light light;
	void Start () {
		InvokeRepeating("Strobe",0,0.07f);
		light = GetComponent<Light>();
	}
	void Strobe()
	{
		light.enabled = !light.enabled;
	}
	

}
