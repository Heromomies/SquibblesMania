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
		}
		else
		{
			Instantiate(objectTransport[inventory[1].objectAcquired],inventory[1].spawnObject.position, 
				inventory[0].spawnObject.rotation, inventory[1].spawnObject);
			inventory[1].objectAcquired += indexObject;
		}

		var randomBloc = Random.Range(0, EventManager.Instance.cleanList.Count - 1);
		var bloc = EventManager.Instance.cleanList[randomBloc].transform;
		Instantiate(objectToSpawn,new Vector3(bloc.position.x, bloc.position.y + 1f, bloc.position.z),
			Quaternion.identity, bloc);
	}

	bool CheckForVictoryConditions(Inventory playerInventory)
	{
		/*for (int i = 0; i < playerInventory.objectAcquired.Count; i++)
		{
		    if (playerInventory.objectAcquired[i].isTeamHasObjet == false)
		    {
		        return false;
		    }
		}*/

		// playerInventory.isAllObjetAcquired = true;
		return true;
	}
}

[Serializable]
public class Inventory
{
	// public bool isAllObjetAcquired;
	public Transform spawnObject;
	public int objectAcquired;
}