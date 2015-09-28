using UnityEngine;
using System.Collections;

public class LeverTrigger : MonoBehaviour {

	bool on = false;

	public delegate void Methods ();
	public Methods stateMethod;
	public GameObject targetObject;

	void Start()
	{
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
		stateMethod();
	}
	public void woodDoorMethod()
	{
		if(on){
			targetObject.GetComponent<Animator>().SetTrigger("switchON");
		}else{
			targetObject.GetComponent<Animator>().SetTrigger("switchOFF");
		}
	}
}
