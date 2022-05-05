using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Snow : MonoBehaviour, IManageEvent
{
	[Header("EVENT SETTINGS")]
	public float heightSpawnParticle;
	[Range(0.0f, 10.0f)] public float timeBeforeRainDisappear;
	[Space]
	public List<GameObject> childrenBlocs;

	[Space] public Transform spawnRain;
	
	private GameObject _particleRain;
	
	public int numberOfBlocsCovered;
	
	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent() // Show the event
	{
		childrenBlocs = GameManager.Instance.cleanList;

		var rand = Random.Range(0, childrenBlocs.Count - numberOfBlocsCovered);

		_particleRain = PoolManager.Instance.SpawnObjectFromPool("ParticleSnowRain", spawnRain.position, Quaternion.identity, spawnRain);

		for (int i = 0; i < numberOfBlocsCovered; i++)
		{
			var childPos = childrenBlocs[i + rand].transform.position;
				
			GameObject snow = PoolManager.Instance.SpawnObjectFromPool("BlocSnow", new Vector3(childPos.x, childPos.y + heightSpawnParticle, childPos.z), Quaternion.identity, childrenBlocs[i + rand].transform);
			snow.transform.localScale = new Vector3(0, 0, 0);

			snow.transform.DOScale(new Vector3(1, 0.1f, 1), 0.5f);
		}

		StartCoroutine(SetActiveFalseGameObjectCoroutine());
	}
	
	IEnumerator SetActiveFalseGameObjectCoroutine()
	{
		yield return new WaitForSeconds(timeBeforeRainDisappear);
		_particleRain.SetActive(false);
		gameObject.SetActive(false);
	}
	public void LaunchEvent() // Launch the event
	{
	}
}
