using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TeamInventoryManager : MonoBehaviour
{
	public Inventory[] inventory;
	public List<GameObject> objectTransport;
	public GameObject objectToSpawn;

	private bool _isFull;

	private static TeamInventoryManager _teamInventoryManager;

	public static TeamInventoryManager Instance => _teamInventoryManager;

	[Header("Objective Display")]
	public GameObject winT1;
	public GameObject winT2;
	public int nbObjectiveT1 = 0;
	public int nbObjectiveT2 = 0;
	public Sprite green;
	// Start is called before the first frame update
	void Awake()
	{
		_teamInventoryManager = this;
	}

	public void AddResourcesToInventory(int indexObject, Player.PlayerTeam playerTeam) // Add resources to the inventory
	{
		if (playerTeam == Player.PlayerTeam.TeamOne && inventory[0].boatObject.Count < 3)
		{
			if (playerTeam == Player.PlayerTeam.TeamOne)
			{
				Instantiate(objectTransport[inventory[0].objectAcquired],inventory[0].spawnObject.position, 
					inventory[0].spawnObject.rotation, inventory[0].spawnObject);
				inventory[0].objectAcquired += indexObject;
			
				inventory[0].boatObject.Add(objectTransport[0]);

				winT1.transform.GetChild(nbObjectiveT1).gameObject.GetComponent<Image>().sprite = green;
				nbObjectiveT1++;
			}
			else
			{
				Instantiate(objectTransport[inventory[1].objectAcquired],inventory[1].spawnObject.position, 
					inventory[0].spawnObject.rotation, inventory[1].spawnObject);
				inventory[1].objectAcquired += indexObject;
				inventory[1].boatObject.Add(objectTransport[0]);

				winT2.transform.GetChild(nbObjectiveT2).gameObject.GetComponent<Image>().sprite = green;
				nbObjectiveT2++;
			}
		
			if (inventory[0].boatObject.Count == 3 || inventory[1].boatObject.Count == 3)
			{
				GameManager.Instance.isConditionVictory = true;
				GameManager.Instance.ShowEndZone();
			}

			inventory[0].boatObject.Add(objectTransport[0]);
		}
		if (playerTeam == Player.PlayerTeam.TeamTwo && inventory[1].boatObject.Count < 3)
		{
			Instantiate(objectTransport[inventory[1].objectAcquired], inventory[1].spawnObject.position,
				inventory[1].spawnObject.rotation, inventory[1].spawnObject);
			inventory[1].objectAcquired += indexObject;
			inventory[1].boatObject.Add(objectTransport[0]);
		}

		if (inventory[0].boatObject.Count == 3 || inventory[1].boatObject.Count == 3)
		{
			GameManager.Instance.isConditionVictory = true;
			GameManager.Instance.ShowEndZone();
		}

		if (inventory[0].boatObject.Count == 3 && inventory[1].boatObject.Count == 3)
		{
			_isFull = true;
		}

		var randomBloc = Random.Range(0, EventManager.Instance.cleanList.Count - 1);
		var bloc = EventManager.Instance.cleanList[randomBloc].transform;

		if (bloc.GetComponent<Node>().isActive && !_isFull)
		{
			Instantiate(objectToSpawn, new Vector3(bloc.position.x, bloc.position.y + 1f, bloc.position.z), Quaternion.identity, bloc);
		}
	}
}

[Serializable]
public class Inventory
{
	public Transform spawnObject;
	public int objectAcquired;
	[HideInInspector] public List<GameObject> boatObject;
}