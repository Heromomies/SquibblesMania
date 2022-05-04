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
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Jump(other.transform);
		}
	}

	void Jump(Transform objectToMove)
	{
		// ReSharper disable once Unity.PreferNonAllocApi
		var position = transform.position;
		var collidersMin = Physics.OverlapSphere(position, radiusMin, layerInteractable); // Detect bloc around the object
		// ReSharper disable once Unity.PreferNonAllocApi
		var collidersMax = Physics.OverlapSphere(position, radiusMax, layerInteractable); // Detect bloc around the object

		var collidersFinished = new List<Collider>();

		collidersFinished.AddRange(collidersMin);

		foreach (var coll in collidersMax)
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

		var randomNumber = Random.Range(0, collidersFinished.Count);

		var posHitInfo = objectToMove.transform.position;

		var xSpawn = (posHitInfo.x + position.x) / 2;
		var zSpawn = (posHitInfo.z + position.z) / 2;

		var listPoint = new List<Vector3>();
		
		listPoint.Add(position);
		listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
		listPoint.Add(collidersFinished[randomNumber].transform.position + new Vector3(0,1,0));

		GameObject trail = PoolManager.Instance.SpawnObjectFromPool("TrailParticleFire", objectToMove.position, Quaternion.identity, objectToMove);
		BezierAlgorithm.Instance.ObjectJumpWithBezierCurve(objectToMove.gameObject, listPoint, speed, animationCurve);

		StartCoroutine(WaitBeforeResetTrail(trail));
	}

	IEnumerator WaitBeforeResetTrail(GameObject trailToDeactivate)
	{
		yield return new WaitForSeconds(3f);
	
		trailToDeactivate.SetActive(false);
	}
}