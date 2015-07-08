using UnityEngine;
using System.Collections;

public class MovePlatform : MonoBehaviour
{
	public float maxX;
	public float minX;
	public float maxY;
	public float minY;

	private Vector3 targetMaxX;
	private Vector3 targetMinX;
	private Vector3 targetMaxY;
	private Vector3 targetMinY;

	private Vector3 targetX;
	private Vector3 targetY;

	public float maxSpeed;
	private float speed;
	//public float maxSpeedY;
	//private float speedY;

	//private bool notSwitchDirX = true; //when switching directions dont fucking apply force normally bitch
	//private bool notSwitchDirY = true;
	private Vector3 startPos;
	public bool moveX;
	public bool moveY;
	void Start(){
		speed = maxSpeed;
		//speedY = maxSpeedY;



		startPos = transform.position;
		targetMaxX = new Vector3 (startPos.x + maxX,startPos.x,startPos.z);
		targetMinX = new Vector3 (startPos.x + minX,startPos.x,startPos.z);
		targetX = targetMaxX;
		targetMaxY = new Vector3 (startPos.x,startPos.y + maxY,startPos.z);
		targetMinY = new Vector3 (startPos.x,startPos.y + minY,startPos.z);
		targetY = targetMaxY;
		//platform  = transform.root.transform;
	}

	//public Transform targetT;
	
	void Update()
	{
		Vector3 pos = transform.position;
		Vector3 moveTo = new Vector3(targetX.x,targetY.y,pos.z);
		transform.position = Vector3.MoveTowards(pos,moveTo,Time.deltaTime*speed);
		if(moveX){
			if(pos.x >= targetMaxX.x ){
				targetX = targetMinX;
			}else if(pos.x <= targetMinX.x ){
				targetX = targetMaxX;
			}
		}
		if(moveY){
			if(pos.y >= targetMaxY.y ){
				//print ("switch");
				targetY = targetMinY;
			}else if(pos.y <= targetMinY.y ){
				targetY = targetMaxY;
			}
		}
		//print (targetY.y+"  "+targetMaxY.y);
	}

}
