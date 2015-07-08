using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Score : MonoBehaviour
{

	public int score = 0;					// The player's score.

	//public GameObject pointsPopupRef;	// A prefab of 100 that appears when the enemy dies.
	private int popupCount = 0;
	List<PointsPopup> popups = new List<PointsPopup>(); 

	private Animator pointsAnim;

	//private Transform playerT;	// Reference to the player control script.
	//private int previousScore = 0;			// The score in the previous frame.
	private int bonusMultiplier = 0 ;
	//public static Score instance;
	public Gun	gunCS;
	private Text txt;



	void Awake ()
	{
		txt=GetComponent<Text>();
		// Setting up the reference.
		//playerT = GameObject.FindGameObjectWithTag("Player").transform;
		gunCS = GameObject.Find("weapon").GetComponent<Gun>();
		makePointsPopups ();
	}


	public void UpdateScore (int scoreAmount,Vector3 scorePos)
	{
		if(bonusMultiplier == 0){
			InvokeRepeating("BonusDeplete",1f,1f);
		}
		bonusMultiplier+=1;

		score += scoreAmount * bonusMultiplier;
		// Set the score text.
		txt.text = "Score: " + score;

		Vector3 pickupPos = new Vector3(scorePos.x,scorePos.y,0);//save the orig pos for pickup

		//scorePos = transform.position;
		//scorePos.y += 1.5f;


		//move score closer?
		//float dist = Vector3.Distance(scorePos,playerT.position)-(Camera.main.transform.position.z/2f);
		//print ("DIST "+popupCount);
		//if(dist >40f){
			//scorePos = Vector3.MoveTowards(scorePos,playerT.position,dist * 0.5f);
		//}

		// Instantiate the 100 points prefab at this point.
		var points = popups [popupCount];//transform.parent.Find ("scorePopup");//(GameObject)Instantiate(hundredPointsUI, scorePos, Quaternion.identity);
		var aString = "pointsSlide"+popupCount.ToString();
		pointsAnim.Play(aString);
		popupCount++;
		if (popupCount >= popups.Count) {
			popupCount = 0;
		}
		//points.transform.Find ("1").GetComponent<MeshRenderer>().sortingLayerName = "UI";
		//points.transform.Find ("00").GetComponent<MeshRenderer>().sortingLayerName = "UI";
		string bString = "";
		if(bonusMultiplier>1){
			bString = "x" + bonusMultiplier.ToString() + "!";
			//TextMesh bonusTxt1 = points.transform.Find ("1").GetComponent<TextMesh>();
			//bonusTxt1.text = bonusMultiplier.ToString();
		}
		//TextMesh bonusTxt = points.transform.Find ("bonus").GetComponent<TextMesh>();
		points.bonusTXT.text = bString;
		points.numTXT.text = scoreAmount.ToString();
		//points.transform.Find ("bonus").GetComponent<MeshRenderer>().sortingLayerName = "UI";
		//points.transform.Find ("bonus").GetComponent<MeshRenderer>().sortingOrder = 5000;

		points.anim.Play ("100points");

		//spawn ammo or health?
		float random = Random.Range(0,10f-(bonusMultiplier)*0.5f); 
		//print (gunCS);
		if(random<2.5f){
			gunCS.SpawnPickup(pickupPos);
		}
		
	}
		// If the score has changed...
		//if(previousScore != score)
			// ... play a taunt.
			//playerControl.StartCoroutine(playerControl.Taunt());

		// Set the previous score to this frame's score.
		//previousScore = score;


	void BonusDeplete()
	{
		//print ("deplete");
		bonusMultiplier--;
		if(bonusMultiplier==0){
			CancelInvoke();
		}
	}
	void makePointsPopups()
	{

		//for( int i = 0; i < 3;i++) {
		Transform par = transform.parent.Find ("pointsPanel");
		pointsAnim = par.GetComponent<Animator>();
		foreach(Transform hn in par){	
			//GameObject hn = (GameObject)Instantiate(pointsPopupRef, transform.position, Quaternion.identity);
			//hn.SetParent(transform.parent,false);
			//hn.GetComponent<MeshRenderer>().sortingLayerName = "UI";
			var p = new PointsPopup(hn.gameObject,hn.Find("num").GetComponent<Text>(),hn.Find("bonus").GetComponent<Text>(),hn.GetComponent<Animator>());
			popups.Add(p);
		}
	}
	public class  PointsPopup
		
	{
		
		public Text numTXT;
		public Text bonusTXT;
		public GameObject pointsGO;
		public Animator anim;
		public PointsPopup(GameObject go,Text nTXT,Text bTXT,Animator a)
		{
			pointsGO = go;
			numTXT = nTXT;
			bonusTXT = bTXT;
			anim = a;
		}
	}
}
