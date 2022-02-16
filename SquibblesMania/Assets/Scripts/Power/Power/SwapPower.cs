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
	private Camera _cam;
	
	public void OnEnable()
	{
		_cam = Camera.main;
		_playerToSwap = GameManager.Instance.currentPlayerTurn.gameObject;
		transform.position = _playerToSwap.transform.position;
		ShowPower();
	}
	
	public void ShowPower() // Show the sphere and admit the player to chose the other player to swap
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, layer);

		switch (hitColliders.Length)
		{
			case 1:
				PowerManager.Instance.ActivateDeactivatePower(2,false);
				PowerManager.Instance.ChangeTurnPlayer();
				break;
			case 2:
				_playerOne = hitColliders[0];
				_playerTwo = hitColliders[1];
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
			if (Input.touchCount > 0)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					Touch touch = Input.GetTouch(0);
					//ray shooting out of the camera from where the touch is

					_playerTwo.transform.position = _cam.ScreenToWorldPoint(touch.position);

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