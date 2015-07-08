using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResetPose : MonoBehaviour {
	//private Color clr;
	//private Transform[] transforms;
	List<Transform> transforms = new List<Transform>();
	List<PosRot> parts = new List<PosRot>(); 
	//public Transform soldier;
	void Start () {

		//transforms = GetComponentsInChildren<Transform>();
		foreach (Transform t in GetComponentsInChildren<Transform>()){
			if(t != transform){
				transforms.Add (t);
				var pr = new PosRot(t.localPosition,t.localRotation);
				parts.Add(pr);
			}

			
		}
		//InvokeRepeating("Pose",2,3);
		//print ("1  "+transforms.Count);
	}
	

	public void Pose (Vector3 p) {
		transform.position = p;

		//print ("Pos  "+transform.position+"  "+p);
		for (int i = 0; i< transforms.Count;i++){
			//print (parts[i].pos);
			transforms[i].localPosition = parts[i].pos;
			transforms[i].localRotation = parts[i].rot;
		
		}

	}
}
public class PosRot
{
	public Vector3 pos;
	public Quaternion rot;
	public PosRot(Vector3 p, Quaternion r)
	{
		pos = p;
		rot = r;
	}
}
