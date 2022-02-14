using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SwapPower : MonoBehaviour
{
	public int range;
	public LayerMask layer;
	
	private GameObject _playerToSwap;
	private Vector3 _pos;
	private Collider _playerOne, _playerTwo;
	private bool _canChoseThePlayer;
	public void OnEnable()
	{
		_playerToSwap = GameManager.Instance.currentPlayerTurn.gameObject;
		transform.position = _playerToSwap.transform.position;
		ShowPower();
	}
	
	public void ShowPower() // Show the sphere and admit the player to chose the other player to swap
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, layer);
		
		_playerOne = hitColliders[0];
		_playerTwo = hitColliders[1];

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
			if (Input.GetMouseButtonDown(0)) // Touch and swap position
			{
				//ray shooting out of the camera from where the mouse is
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out var hit, range, layer))
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

	void SwapPosition(Transform playerOne, Transform playerTwo) // Swap the position between the two players
	{
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;
		PowerManager.Instance.ActivateDeactivatePower(2,false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
}