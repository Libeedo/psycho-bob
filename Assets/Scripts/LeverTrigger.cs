using UnityEngine;
using System.Collections;

public class LeverTrigger : MonoBehaviour {

	bool on = false;

	public delegate void Methods ();
	public Methods stateMethod;
	public GameObject targetObject;
	//private AudioSource leverAud;
	void Start()
	{
		//leverAud = transform.parent.GetComponent<AudioSource>();
		stateMethod = woodDoorMethod;
	}

	public void TriggerLever()
	{
		//print (on);
		if(on){
			transform.parent.GetComponent<Animator>().Play ("LeverSwitchOFF");
			on = false;
		}else{
			transform.parent.GetComponent<Animator>().Play ("LeverSwitchON");
			on = true;
		}
		//leverAud.Play ();
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
