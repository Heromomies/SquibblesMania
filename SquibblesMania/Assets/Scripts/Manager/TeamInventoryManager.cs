using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TeamInventoryManager : MonoBehaviour
{
	public Inventory[] inventory;
	public List<GameObject> objectTransport;
	public GameObject objectToSpawn;

	private static TeamInventoryManager _teamInventoryManager;

	public static TeamInventoryManager Instance => _teamInventoryManager;

	// Start is called before the first frame update
	void Awake()
	{
		_teamInventoryManager = this;
	}

	public void AddResourcesToInventory(int indexObject, Player.PlayerTeam playerTeam)
	{
		if (playerTeam == Player.PlayerTeam.TeamOne)
		{
			Instantiate(objectTransport[inventory[0].objectAcquired],inventory[0].spawnObject.position, 
				inventory[0].spawnObject.rotation, inventory[0].spawnObject);
			inventory[0].objectAcquired += indexObject;
			
			inventory[0].boatObject.Add(objectTransport[0]);
		}
		else
		{
			Instantiate(objectTransport[inventory[1].objectAcquired],inventory[1].spawnObject.position, 
				inventory[0].spawnObject.rotation, inventory[1].spawnObject);
			inventory[1].objectAcquired += indexObject;
			inventory[1].boatObject.Add(objectTransport[0]);
		}

		var randomBloc = Random.Range(0, EventManager.Instance.cleanList.Count - 1);
		var bloc = EventManager.Instance.cleanList[randomBloc].transform;

		if (bloc.GetComponent<Node>().isActive)
		{
			Instantiate(objectToSpawn,new Vector3(bloc.position.x, bloc.position.y + 1f, bloc.position.z),
				Quaternion.identity, bloc);
		}
		else
		{
			AddResourcesToInventory(0, Player.PlayerTeam.None);
		}
		

		if (inventory[0].boatObject.Count == 3 || inventory[1].boatObject.Count == 3)
		{
			GameManager.Instance.isConditionVictory = true;
			GameManager.Instance.ShowEndZone();
		}
	}
	
}

[Serializable]
public class Inventory
{
	// public bool isAllObjetAcquired;
	public Transform spawnObject;
	public int objectAcquired;
	[HideInInspector] public List<GameObject> boatObject;
}