using UnityEngine;
using System.Collections;

public class NursePoint : MonoBehaviour {

	private Vector2 nursePos;
	
	void OnTriggerEnter2D (Collider2D col)
	{
		print (col.name);
		if(col.tag == "Nurse")
		{
			nursePos = col.transform.position;
			nursePos.y += 3.5f;
			StartCoroutine("DelayEnd");
			Level.instance.WinLevel();

		}
	}
	IEnumerator DelayEnd()
	{
		yield return new WaitForSeconds(2);
		transform.root.transform.GetComponent<Animator>().Play ("spawnPoint spawn");
		Level.instance.makeTeleFlash(nursePos);
		Destroy (gameObject);
	}
}
