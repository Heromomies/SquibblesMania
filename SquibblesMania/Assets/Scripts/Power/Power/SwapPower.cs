using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SwapPower : MonoBehaviour, IManagePower
{
	public int range;
	public LayerMask layer;
	
	private GameObject _playerToSwap;
	private Vector3 _pos;
	private Collider _playerOne, _playerTwo;
	private bool _canChoseThePlayer;
	public void Start()
	{
		_playerToSwap = GameManager.Instance.currentPlayerTurn.gameObject;
		transform.position = _playerToSwap.transform.position;
		ShowPower();
	}
	
	public void ShowPower()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, layer);
		
		_playerOne = hitColliders[0];
		_playerTwo = hitColliders[1];

		/*foreach (var col in hitColliders)
		{
			col.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
		}*/
		
		switch (hitColliders.Length)
		{
			case 1:
				Debug.Log("I'm null and i can't do anything");
				break;
			case 2:
				LaunchPower();
				break;
		}

		if (hitColliders.Length >= 3)
		{
			_canChoseThePlayer = true;
		}
	}

	private void Update()
	{
		if (_canChoseThePlayer)
		{
			if (Input.GetMouseButtonDown(0))
			{
				//ray shooting out of the camera from where the mouse is
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out var hit))
				{
					//print out the name if the raycast hits something
					_playerTwo = hit.collider;
					
					LaunchPower();
				}
			}
		}
	}
	
	public void LaunchPower()
	{
		_pos = _playerOne.transform.position;
		SwapPosition(_playerOne.transform, _playerTwo.transform);
	}

	void SwapPosition(Transform playerOne, Transform playerTwo)
	{
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;
		PowerManager.Instance.ActivateDeactivatePower(false);
	}
}