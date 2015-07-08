using UnityEngine;
using System.Collections;
public class Soldier_Sprites : MonoBehaviour 
{
	public static Soldier_Sprites S;

	public  Sprite[] heads;
	public  Sprite[] bodies;
	
	void Awake()
	{
		if(S != null)
			GameObject.Destroy(S);
		else
			S = this;
		
		DontDestroyOnLoad(this);
	}
	public Sprite getHead(int num)
	{
		return heads[num];
	}
	public Sprite getBody(int num)
	{
		return bodies[num];
	}
}
