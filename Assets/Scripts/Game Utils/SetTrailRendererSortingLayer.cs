using UnityEngine;
using System.Collections;

public class SetTrailRendererSortingLayer : MonoBehaviour
{
	public string sortingLayerName;		// The name of the sorting layer the particles should be set to.
	public int sortingOrder;

	void Start ()
	{
		// Set the sorting layer of the particle system.
		GetComponent<TrailRenderer>().sortingLayerName = sortingLayerName;
		GetComponent<TrailRenderer>().sortingOrder = sortingOrder;
	}

}
