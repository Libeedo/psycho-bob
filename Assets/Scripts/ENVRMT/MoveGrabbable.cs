using UnityEngine;
using System.Collections;

public class MoveGrabbable : MonoBehaviour
{

	public float maxX;
	public float minX;
	public float speedX;
	public float maxY;
	public float minY;
	public float speedY;
	public bool flipX = false; 
	private Vector2 startPos;
	void Start(){
		startPos = transform.position;
		//platform  = transform.root.transform;
	}


	
	void FixedUpdate()
	{

		Vector2 pos = transform.position;
		pos.x += speedX;
		pos.y += speedY;
		if((pos.x - startPos.x) > maxX || ( startPos.x - pos.x) > minX){
			speedX = -speedX;
			if(flipX){
				Flip();
			}
		}
		if((pos.y - startPos.y) > maxY || ( startPos.y - pos.y) > minY){
			//transform.rigidbody2D.velocity = Vector2.zero;
			speedY = -speedY;
		}
		transform.position = pos;
		//transform.rigidbody2D.AddTorque(40f);
	}

	void Flip ()
	{

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		
		
		
	}



}
