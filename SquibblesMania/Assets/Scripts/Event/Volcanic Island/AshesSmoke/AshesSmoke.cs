using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AshesSmoke : MonoBehaviour, IManageEvent
{
	[Header("EVENT")] private List<GameObject> _cubeOnMap;
	[HideInInspector] public List<GameObject> cubeTouched;
	public List<GameObject> parentBlocs;

	public GameObject ashes;
	public float heightSpawnParticle;
	private int _cubeToChange;
	public List<GameObject> childrenBlocs;
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

	/// <summary>
	/// To random a list easily
	/// </summary>
	/*void Shuffle<T>(List<T> inputList)
	{
		for (int i = 0; i < inputList.Count; i++)
		{
			T temp = inputList[i];
			int random = Random.Range(1, inputList.Count);
			inputList[i] = inputList[random];
			inputList[random] = temp;
		}
	}*/
	
	public void ShowEvent() // Show the event
	{
		foreach (var t in parentBlocs)
		{
			foreach (var child in t.GetComponentsInChildren<Node>())
			{
				childrenBlocs.Add(child.gameObject);
			}
		}

		int rand = Random.Range(0, childrenBlocs.Count 
		                           - conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered);

		
		for (int i = 0; i < conditionsDangerousnessAshesSmoke[EventManager.Instance.dangerousness].numberOfBlocsCovered; i++)
		{
			var c = childrenBlocs[i + rand].transform.position;
			Instantiate(ashes, new Vector3(c.x, c.y + 0.55f, c.z), Quaternion.identity);
		}
		
		gameObject.SetActive(false);
	}

	public void LaunchEvent() // Launch the event
	{
		
	}
}