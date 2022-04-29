using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PolarWind : MonoBehaviour, IManageEvent
{
	public int hideRaycastDistance;
	
	public TextMeshProUGUI windIsComing;
	
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	
	public void ShowEvent()
	{
		windIsComing.gameObject.SetActive(true);
	}

	public void LaunchEvent()
	{
		
	}
}