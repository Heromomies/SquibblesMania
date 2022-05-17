using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireJump : MonoBehaviour
{
	public int radiusMin, radiusMax;
	public LayerMask layerInteractable;
	public float ySpawn;
	public float speed;
	public AnimationCurve animationCurve;
	
	private Transform _objectToMove, _objectToGo;
	private float _speedLookAt = 3f;
	private bool _canLook;
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !NFCManager.Instance.powerActivated)
		{
			AudioManager.Instance.Play("PlayerBurn");
			Jump(other.transform);
		}
	}

	void Jump(Transform objectToMove)
	{
		var position = transform.position;
		
		// ReSharper disable once Unity.PreferNonAllocApi
		var collidersMin = Physics.OverlapSphere(position, radiusMin, layerInteractable); // Detect bloc around the object
		// ReSharper disable once Unity.PreferNonAllocApi
		var collidersMax = Physics.OverlapSphere(position, radiusMax, layerInteractable); // Detect bloc around the object

		var collidersFinished = new List<Collider>();

		collidersFinished.AddRange(collidersMin);

		foreach (var coll in collidersMax)
		{
			if (TryGetComponent(out Node node))
			{
				if (node.isActive)
				{
					if (collidersFinished.Contains(coll))
					{
						collidersFinished.Remove(coll);
					}
					else
					{
						collidersFinished.Add(coll);
					}
				}
				else
				{
					collidersFinished.Remove(coll);
				}
			}
		}

		var randomNumber = Random.Range(0, collidersFinished.Count);

		var posHitInfo = objectToMove.transform.position;

		var xSpawn = (posHitInfo.x + position.x) / 2;
		var zSpawn = (posHitInfo.z + position.z) / 2;

		var listPoint = new List<Vector3>();
		
		listPoint.Add(position);
		listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
		listPoint.Add(collidersFinished[randomNumber].transform.position + new Vector3(0,1,0));

		GameManager.Instance.currentPlayerTurn.playerAnimator.SetBool("isBurning", true);

		_objectToMove = objectToMove;
		_objectToGo = collidersFinished[randomNumber].transform;
		_canLook = true;
		
		GameObject trail = PoolManager.Instance.SpawnObjectFromPool("TrailParticleFire", objectToMove.position, Quaternion.identity, objectToMove);
		BezierAlgorithm.Instance.ObjectJumpWithBezierCurve(objectToMove.gameObject, listPoint, speed, animationCurve);

		StartCoroutine(WaitBeforeResetTrail(trail));
	}

	private void Update()
	{
		if (_canLook)
		{
			var lookPos = _objectToGo.position - _objectToMove.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation(lookPos);
			_objectToMove.rotation = Quaternion.Slerp(_objectToMove.rotation, rotation, Time.deltaTime * _speedLookAt);
		}
	}

	IEnumerator WaitBeforeResetTrail(GameObject trailToDeactivate)
	{
		yield return new WaitForSeconds(1f);
		
		GameManager.Instance.currentPlayerTurn.playerAnimator.SetBool("isBurning", false);
		_canLook = false;
		yield return new WaitForSeconds(2f);
	
		trailToDeactivate.SetActive(false);
	}
}