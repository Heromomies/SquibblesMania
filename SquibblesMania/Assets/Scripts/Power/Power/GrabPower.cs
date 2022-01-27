using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPower : MonoBehaviour, IManagePower
{
	public int grabRange;


	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	void Start()
	{
	}

	public void ButtonClickedGrab(int numberDirectionVector)
	{
		//Debug.Log(numberDirectionVector);
		RaycastHit hit;
		
		if (Physics.Raycast( transform.position, _vectorRaycast[numberDirectionVector], out hit, grabRange))
		{
			if (hit.collider.gameObject.layer == 3)
			{
				transform.position = hit.collider.transform.position - _vectorRaycast[numberDirectionVector];
			}
			else if (hit.collider.gameObject.layer == 6)
			{
				
			}
		}
	}
	public void ShowPower()
	{
		
	}

	public void LaunchPower()
	{
	}
}