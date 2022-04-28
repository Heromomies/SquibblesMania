using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarWind : MonoBehaviour, IManageEvent
{
	
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	
	public void ShowEvent()
	{
		
	}

	public void LaunchEvent()
	{
		
	}

	public void LaunchRaycastToDetectIfPlayersAreHide()
	{
		foreach (var player in GameManager.Instance.players)
		{
			
		}
	}
}