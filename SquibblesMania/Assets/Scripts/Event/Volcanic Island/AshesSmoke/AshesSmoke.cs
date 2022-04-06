using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AshesSmoke : MonoBehaviour, IManageEvent
{
	[Header("EVENT SETTINGS")]
	public float heightSpawnParticle;[Space]

	public List<GameObject> childrenBlocs;
	
	[Header("CONDITIONS DANGEROUSNESS")]
	public Conditions[] conditionsDangerousnessAshesSmoke;

	[Serializable]
	public struct Conditions
	{
		public int numberOfBlocsCovered;
	}

	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent() // Show the event
	{
		childrenBlocs = EventManager.Instance.cleanList;

		var rand = Random.Range(0, childrenBlocs.Count
		                           - conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered);

		AudioManager.Instance.Play("VolcanoSmoking");
		
		for (int i = 0; i < conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered; i++)
		{
			if (childrenBlocs[i + rand].CompareTag("BlackBlock"))
			{
				i++;
			}
			else
			{
				var childPos = childrenBlocs[i + rand].transform.position;
				
				GameObject ashe = PoolManager.Instance.SpawnObjectFromPool("Ashe", new Vector3(childPos.x, childPos.y + heightSpawnParticle, childPos.z), Quaternion.identity, childrenBlocs[i + rand].transform);
				ashe.transform.localScale = new Vector3(0, 0, 0);

				ashe.transform.DOScale(new Vector3(1, 0.1f, 1), 0.5f);
			}
		}

		StartCoroutine(SetActiveFalseGameObjectCoroutine());
	}
	
	IEnumerator SetActiveFalseGameObjectCoroutine()
	{
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}
	public void LaunchEvent() // Launch the event
	{
	}
}