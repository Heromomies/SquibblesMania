using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AshesSmoke : MonoBehaviour, IManageEvent
{
	[Header("EVENT SETTINGS")] 
	public List<GameObject> parentBlocs; [Space]
	public GameObject ashes;[Space]
	public float heightSpawnParticle;[Space]

	[HideInInspector] public List<GameObject> childrenBlocs;
	
	[Header("CONDITIONS DANGEROUSNESS")]
	public Conditions[] conditionsDangerousnessAshesSmoke;

	[Serializable]
	public struct Conditions
	{
		public int numberOfBlocsCovered;
	}

	private void OnEnable()
	{
		Shuffle(parentBlocs);
		ShowEvent();
	}

	/// <summary>
	/// To random a list easily
	/// </summary>
	void Shuffle<T>(List<T> inputList)
	{
		for (int i = 0; i < inputList.Count; i++)
		{
			T temp = inputList[i];
			int random = Random.Range(1, inputList.Count);
			inputList[i] = inputList[random];
			inputList[random] = temp;
		}
	}
	public void ShowEvent() // Show the event
	{
		foreach (var parentB in parentBlocs)
		{
			foreach (var child in parentB.GetComponentsInChildren<Node>())
			{
				childrenBlocs.Add(child.gameObject);
			}
		}

		var rand = Random.Range(0, childrenBlocs.Count
		                           - conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered);

		for (int i = 0; i < conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered; i++)
		{
			if (childrenBlocs[i + rand].CompareTag("Black Block"))
			{
				i--;
			}
			else
			{
				var childPos = childrenBlocs[i + rand].transform.position;

				GameObject ashe =Instantiate(ashes, new Vector3(childPos.x, childPos.y + heightSpawnParticle, childPos.z), Quaternion.identity);
				ashe.transform.localScale = new Vector3(0, 0, 0);

				ashe.transform.DOScale(new Vector3(1, 0.1f, 1), 0.5f);
			}
		}

		gameObject.SetActive(false);
	}

	public void LaunchEvent() // Launch the event
	{
	}
}