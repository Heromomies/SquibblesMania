using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EarthQuakeEvent : MonoBehaviour, IManageEvent
{
	[Header("EVENT SETTINGS")]
	public int radius;
	[Space]
	public LayerMask layer;
	
	public Collider[] colliders;

	[Header("PARENT")]
	[Space]
	public Transform mapParent;
	public GameObject blocParent;

	private Camera _cam;
	[Space]
	[Header("CONDITIONS DANGEROUSNESS")]
	
	public Conditions[] conditionsDangerousnessEarthQuake;

	[Serializable]
	public struct Conditions
	{
		public int numberOfBlocsTouched;
	}

	private void OnEnable()
	{
		_cam = Camera.main;
		ShowEvent();
	}

	public void ShowEvent()
	{
		colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layer); // Detect bloc around the object
		
		AudioManager.Instance.Play("VolcanoShaking");
		
		for (int i = 0; i < conditionsDangerousnessEarthQuake[EventManager.Instance.dangerousness].numberOfBlocsTouched; i++) // Set the position of random blocs touched in Y equal to 0
		{
			int randomNumber = Random.Range(0, colliders.Length);
			if (Math.Abs(colliders[randomNumber].transform.position.y - 1) < 0.1f || colliders[randomNumber].CompareTag("BlackBlock")) // If the Y position is equal to 0, add one bloc to touch
			{
				i--;
			}
			else // When a bloc can be moved 
			{
				var col = colliders[randomNumber].transform.position;
				
				GameObject parent = Instantiate(blocParent, mapParent);
				colliders[randomNumber].transform.parent = parent.transform;
			
				col = new Vector3(col.x, 1, col.z);
				colliders[randomNumber].transform.DOMove(col, 5f);
				//_cam.DOShakePosition(5f, 0.1f, 100);
			}
		}
		
		StartCoroutine(SetActiveFalseBullet());
	}
	IEnumerator SetActiveFalseBullet()
	{
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}
	public void LaunchEvent()
	{
	}
}