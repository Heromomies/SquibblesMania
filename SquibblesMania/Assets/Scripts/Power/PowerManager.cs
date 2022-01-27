using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
	public int maxDistance;
	public List<GameObject> powers;
	
	[Space] [Header("UI")] public Button buttonOne;
	public Button buttonTwo;

	[Space] [Header("VECTORS")] public List<Vector3> vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	
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
		Debug.DrawRay(transform.position, Vector3.back * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.forward * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.right * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position, Vector3.left * maxDistance, Color.yellow);
	}

	public void PlayerChangeTurn()
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
		
		RaycastHit hit;
		
		for (int i = 0; i < vectorRaycast.Count; i++)
		{
			if (Physics.Raycast(transform.position, vectorRaycast[i], out hit, maxDistance))
			{
				//Debug.Log("i'm actually touching" + hit.collider.name);
				
				buttonOne.interactable = true;
				buttonTwo.interactable = true;
			}
		}
	}

	public void ClickButtonToLaunchPower(int powerNumber)
	{
		switch (powerNumber)
		{
			case 0 : powers[0].SetActive(true);
				break;
			case 1 : powers[1].SetActive(true);
				break;
			case 2 : powers[2].SetActive(true);
				break;
			case 3 : powers[3].SetActive(true);
				break;
		}
	}

	public void DeactivatePower(int powerNumberCoroutine)
	{
		switch (powerNumberCoroutine)
		{
			case 0 : powers[0].SetActive(false);
				break;
			case 1 : powers[1].SetActive(false);
				break;
			case 2 : powers[2].SetActive(false);
				break;
			case 3 : powers[3].SetActive(false);
				break;
		}
	}
	
	public void ButtonClicked()
	{
		buttonOne.interactable = false;
		buttonTwo.interactable = false;
	}
}