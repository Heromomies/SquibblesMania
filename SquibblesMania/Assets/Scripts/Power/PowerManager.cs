using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
	public int maxDistance;
	public GameObject buttonOne, buttonTwo;

	[HideInInspector] public Transform targetTouched;

	private int _raycastPos;

	#region Singleton

	private static PowerManager powerManager;

	public static PowerManager Instance => powerManager;
	// Start is called before the first frame update

	private void Awake()
	{
		powerManager = this;
		targetTouched.position = GameManager.Instance.currentPlayerTurn.transform.position;
	}

	#endregion

	private void Start()
	{
		targetTouched.position = GameManager.Instance.currentPlayerTurn.transform.position;
	}

	private void Update()
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;

		Debug.DrawRay(transform.position + new Vector3(0f, 0f, -0.6f), Vector3.back * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(0f, 0f, 0.6f), Vector3.forward * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(-0.6f, 0f, 0), Vector3.left * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position + new Vector3(0.6f, 0f, 0), Vector3.right * maxDistance, Color.yellow);

		if (Physics.Raycast(transform.position + new Vector3(0.5f, 0f, 0f), Vector3.back, out var hit, maxDistance))
		{
			_raycastPos = 0;
			targetTouched.position = hit.transform.position;

			buttonOne.SetActive(true);
			buttonTwo.SetActive(true);
		}

		if (Physics.Raycast(transform.position, Vector3.forward, out hit, maxDistance))
		{
			_raycastPos = 1;
			targetTouched.position = hit.transform.position;

			buttonOne.SetActive(true);
			buttonTwo.SetActive(true);
		}

		if (Physics.Raycast(transform.position, Vector3.left, out hit, maxDistance))
		{
			_raycastPos = 2;
			targetTouched.position = hit.transform.position;

			buttonOne.SetActive(true);
			buttonTwo.SetActive(true);
		}

		if (Physics.Raycast(transform.position, Vector3.right, out hit, maxDistance))
		{
			_raycastPos = 3;
			targetTouched.position = hit.transform.position;

			buttonOne.SetActive(true);
			buttonTwo.SetActive(true);
		}
	}

	public void Attract()
	{
		GetComponentInChildren<AttractionPower>().LaunchPower(_raycastPos);
	}
}