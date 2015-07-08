using UnityEngine;
using System.Collections;

public class HitZone : MonoBehaviour {

	public delegate void HitDel(float dmg, bool blownup);
	public HitDel hitDel;

	public bool pilotMode = false;


	public enum ObectType{
		COWMAN,
		HELICOPTER,
		PILOT
	}
	public ObectType objectType;

	void Start()
	{
			if (objectType == ObectType.COWMAN) {
					hitDel = transform.root.GetComponent<Boss_cowMan> ().Hurt;
			} else if (objectType == ObectType.HELICOPTER) {
					hitDel = transform.root.GetComponent<Vehicle> ().Hurt;
			} else if (objectType == ObectType.PILOT) {
				hitDel = transform.root.GetComponent<Vehicle> ().PilotHurt;
			}
	}
}
