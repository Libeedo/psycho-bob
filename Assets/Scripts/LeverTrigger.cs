using UnityEngine;
using System.Collections;

public class LeverTrigger : MonoBehaviour {

	bool on = false;

	public delegate void Methods ();
	public Methods stateMethod;
	public GameObject targetObject;
	private AudioSource leverAud;
	void Start()
	{
		leverAud = transform.root.GetComponent<AudioSource>();
		stateMethod = woodDoorMethod;
	}

	public void TriggerLever()
	{
		//print (on);
		if(on){
			transform.root.transform.GetComponent<Animator>().Play ("LeverSwitchOFF");
			on = false;
		}else{
			transform.root.transform.GetComponent<Animator>().Play ("LeverSwitchON");
			on = true;
		}
		leverAud.Play ();
		stateMethod();
	}
	public void woodDoorMethod()
	{
		targetObject.GetComponent<AudioSource>().Play ();
		if(on){
			targetObject.GetComponent<Animator>().SetTrigger("switchON");
		}else{
			targetObject.GetComponent<Animator>().SetTrigger("switchOFF");
		}
	}
}
