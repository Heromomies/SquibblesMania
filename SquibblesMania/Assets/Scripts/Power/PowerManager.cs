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
	private RaycastHit _hitRaycasta;

	#region Singleton

	private static PowerManager powerManager;

	public static PowerManager Instance => powerManager;
	// Start is called before the first frame update

	private void Awake()
	{
		powerManager = this;
	}

	#endregion

	public void Update()
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
		Debug.DrawRay(transform.position, Vector3.back * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.forward * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.right * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.left * maxDistance, Color.yellow);
	}

	public void RaycastEvent()
	{
		RaycastHit hit;
		

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
		hitRaycast = new RaycastHit();
		
		buttonOne.interactable = false;
		buttonTwo.interactable = false;
	}
}