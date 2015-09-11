using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	public bool selected = false;
	public Material[] bulbMats;
	private MeshRenderer bulbRen;
	/*void Start()
	{
		bulbRen = transform.Find ("legs").transform.Find ("joint14").transform.Find ("joint13").transform.Find ("lightbulb").transform.Find ("pSphere1").GetComponent<MeshRenderer> ();
	}*/
	private Vector3 teleFlashPos;
	void Start()
	{
		teleFlashPos = transform.position;
		teleFlashPos.y += 5;
	}
	public void EnableSpawn()
	{
		if(selected){return;}
		Level.instance.enableSpawn(this);
		transform.Find ("spawnTrigger").GetComponent<AudioSource>().Play();
		transform.Find ("light").gameObject.SetActive(true);
		//bulbRen.material = bulbMats [1];
		selected = true;
		
		//audio.Play();

	}
	public void DisableSpawn()
	{
		selected = false;
		transform.Find ("light").gameObject.SetActive(false);
		//bulbRen.material = bulbMats [0];
	}
	public void Spawn()
	{
		//print ("spawn");
		GetComponent<AudioSource>().Play ();
		GetComponent<Animator>().Play("spawnPoint spawn");
		Level.instance.makeTeleFlash(teleFlashPos);
		//transform.Find("skirt").GetComponent<Animator>().Play("skirtSpawn");
	}
}
