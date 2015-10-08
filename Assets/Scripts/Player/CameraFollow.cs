using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	private float xMargin = 2f;		// Distance in the x axis the player can move before the camera follows.
	private float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	private float xSmooth = 3f;		// How smoothly the camera catches up with it's target movement in the x axis.
	private float ySmooth = 10f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.

	public bool deathPan = true;
	private float startZ;
	public float levelZoom = 0f;
	private float currentLevelZoom = 0f;
	private Transform player;		// Reference to the player's transform.

	private Transform xhair;
	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag("Player").transform;
		//float[] vals = {maxXAndY.x,minXAndY.x+10f};
		//player.Find ("weapon").GetComponent<Gun>().SetCameraMaxMin(vals);

	}
	void Start()
	{
		startZ = transform.position.z;
		xhair =GameObject.Find("xhair").transform;
	}

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}


	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}
	void FixedUpdate()
	{


		TrackCrosshair();
		TrackPlayer();
	}



	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = player.position.x;//transform.position.x;
		float targetY = player.position.y;//transform.position.y;

		// If the player has moved beyond the x margin...
		if(CheckXMargin()){
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);

		}
		// If the player has moved beyond the y margin...
		if(CheckYMargin()){
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
			targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);
		}
	
		//Vector3 mousePos = mouseZoom();

		if(deathPan){
			//mousePos = Vector3.zero;
			if( Mathf.Abs(transform.position.x - player.position.x) <8f){
				if(Mathf.Abs(transform.position.y - player.position.y) <8f){
					print("death pan over");
					deathPan = false;
					player.gameObject.SetActive(true);
					player.GetComponent<PlayerHealth>().Spawn();
					GetComponent<AudioListener>().enabled = false;
					
				}
			}
			
		}

		//targetX += mousePos.x;
		//targetY += mousePos.y;
		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
		//float targetZ = levelZoom;
		//float diff = transform.position.z -startZ;
		if(Mathf.Abs (levelZoom - currentLevelZoom) > 0.1f){

			currentLevelZoom = Mathf.Lerp(currentLevelZoom, levelZoom,3f * Time.deltaTime);
			

		}
		//print ("zoooom"+levelZoom+"  "+currentLevelZoom);
		float vel = startZ-currentLevelZoom;//speedZoom ();




		// Set the camera's position to the target position with the same z component.
		Vector3 pos = new Vector3(targetX, targetY, vel);
		transform.position = Vector3.Lerp(transform.position,pos,60f * Time.deltaTime);
		//transform.position = new Vector3(targetX, targetY, transform.position.z);
		//Vector3 ps = transform.position;

		//transform.LookAt(xhair);
	}
	private void TrackCrosshair()
	{

		//Vector3 mouse = Input.mousePosition;
		//mouse.z = -transform.Find ("dummyCam").position.z;
		
		//var mousePos = transform.Find ("dummyCam").camera.ScreenToWorldPoint (mouse);
		
		//Vector3 v = new Vector3(mousePos.x,mousePos.y,-2f);
		//xhair.transform.position = Vector3.Lerp(xhair.transform.position,v,200f * Time.deltaTime);

		//rotate camera towards crosshair slightly
		//Vector3 vn = xhair.transform.position - transform.position;//player.position;

		//Vector3 rot = new Vector3(-vn.y/2f, vn.x/2f, 0);
		//Quaternion cAngle3 = Quaternion.LookRotation(vn);//xhair.position - transform.position);//Euler(rot);

		Vector3 vn = (xhair.transform.position - transform.position);
		//float vnX = Mathf.Lerp(transform.localEulerAngles.x, -vn.y/2.5f, 5f * Time.deltaTime);
		Vector3 rot = new Vector3(-vn.y/2f, vn.x/2f, 0);
		//Vector3 rot = new Vector3(vnX, vn.x/3f, 0);
		Quaternion cAngle3 = Quaternion.Euler(rot);
		//transform.rotation = cAngle3;
		//Camera.main.
		transform.rotation = Quaternion.Lerp(transform.rotation, cAngle3,50*Time.deltaTime);//cAngle3;
		//transform.LookAt(xhair);
	}
	public float[] GetCameraMaxMin()
	{
		float[] vals = {maxXAndY.x,minXAndY.x+10f};
		return vals;
	}
	/*float speedZoom()
	{
		///////////////////Vector2 vel = new Vector2 (Mathf.Abs(player.rigidbody2D.velocity.x),Mathf.Abs(player.rigidbody2D.velocity.y));
		float vel = Mathf.Abs(player.rigidbody2D.velocity.x)+Mathf.Abs(player.rigidbody2D.velocity.y);
		float z = startZ - vel/1.5f;
		float targetZ = Mathf.Lerp(transform.position.z, z, ySmooth * Time.deltaTime);
		return targetZ;
	}

	Vector3 mouseZoom()
	{
		Vector3 mouse = Input.mousePosition;
		float z = -Camera.main.transform.position.z;
		mouse.z = z;
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (mouse);
		Vector3 angle = (mousePos - transform.position).normalized;
		return angle;
	}*/
}
