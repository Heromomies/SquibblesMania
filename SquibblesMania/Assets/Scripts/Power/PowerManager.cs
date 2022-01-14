using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
	public int maxDistance;
	[Space] [Header("UI")] public Button buttonOne;
	public Button buttonTwo;

	[Space] [Header("VECTORS")] public List<Vector3> vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	[HideInInspector] public int raycastPos;
	[HideInInspector] public RaycastHit hitRaycast;

	#region Singleton

	private static PowerManager powerManager;

	public static PowerManager Instance => powerManager;
	// Start is called before the first frame update

	private void Awake()
	{
		powerManager = this;
	}

	#endregion

	public void RaycastEvent()
	{
		RaycastHit hit;
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
		
		for (int i = 0; i < vectorRaycast.Count; i++)
		{
			if (Physics.Raycast(transform.position, vectorRaycast[i], out hit, maxDistance))
			{
				raycastPos = i;
				hitRaycast = hit;
				buttonOne.interactable = true;
				buttonTwo.interactable = true;
			}
		}
	}

	public void ButtonClicked()
	{
		buttonOne.interactable = false;
		buttonTwo.interactable = false;
	}
}