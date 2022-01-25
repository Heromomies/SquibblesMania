using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapPower : MonoBehaviour
{
	public int range;
	private GameObject _playerToSwap;
	public LayerMask layer;
	private Vector3 _pos;

	// Update is called once per frame
	void Update()
	{
		_playerToSwap = GameManager.Instance.currentPlayerTurn.gameObject;

		transform.position = _playerToSwap.transform.position;
		
		if (Input.GetKeyDown(KeyCode.E))
		{
			Collide();
		}
	}

	public void Collide()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, layer);
		foreach (var hitCollider in hitColliders)
		{
			Debug.Log(hitCollider.name);
		}
		
		_pos = hitColliders[0].transform.position;
		SwapPosition(hitColliders[0].transform, hitColliders[1].transform);

	}
	void SwapPosition(Transform playerOne, Transform playerTwo)
	{
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}
}