using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TeamInventoryManager : MonoBehaviour
{
	[Header("Balloon")] public Inventory[] inventory;
	public List<GameObject> objectTransport;
	public GameObject objectToSpawn;
	private GameObject _balloonT1;
	private GameObject _balloonT2;

	private bool _isFull;

	private static TeamInventoryManager _teamInventoryManager;

	public static TeamInventoryManager Instance => _teamInventoryManager;

	[Header("Objective Display")]
	private GameObject _lastObj;
	// Start is called before the first frame update

	 void Start()
	 {
	     _balloonT1 = Instantiate(objectTransport[3], inventory[0].spawnObject.position, inventory[0].spawnObject.rotation, inventory[0].spawnObject);
	     _balloonT2 = Instantiate(objectTransport[4], inventory[1].spawnObject.position, inventory[1].spawnObject.rotation, inventory[1].spawnObject);
	 }
	void Awake()
	{
		_teamInventoryManager = this;
	}

	public void AddResourcesToInventory(int indexObject, Player.PlayerTeam playerTeam) // Add resources to the inventory
	{
		if (playerTeam == Player.PlayerTeam.TeamOne && inventory[0].objectAcquired < 3)
		{
			Instantiate(objectTransport[inventory[0].objectAcquired], inventory[0].spawnObject.position, inventory[0].spawnObject.rotation, inventory[0].spawnObject);
			//inventory[0].imageConditions[inventory[0].objectAcquired].color = Color.green;
			inventory[0].uiWinConditionsList[inventory[0].objectAcquired].gameObject.SetActive(true);
			inventory[0].objectAcquired += indexObject;
			
			Destroy(_balloonT1.gameObject.transform.GetChild(0).gameObject);
		}

		if (playerTeam == Player.PlayerTeam.TeamTwo && inventory[1].objectAcquired < 3)
		{
			Instantiate(objectTransport[inventory[1].objectAcquired], inventory[1].spawnObject.position, inventory[1].spawnObject.rotation, inventory[1].spawnObject);
			inventory[1].imageConditions[inventory[1].objectAcquired].color = Color.green;
			inventory[1].objectAcquired += indexObject;
			Destroy(_balloonT2.gameObject.transform.GetChild(0).gameObject);
		}

		if (inventory[0].objectAcquired == 3 || inventory[1].objectAcquired == 3)
		{
			GameManager.Instance.isConditionVictory = true;
			GameManager.Instance.ShowEndZone();
		}

		if (inventory[0].objectAcquired == 3 && inventory[1].objectAcquired == 3)
		{
			_isFull = true;
		}
		
		do
		{
			if(_lastObj != null)
				GameManager.Instance.cleanList.Add(_lastObj);
			
			var randomBloc = Random.Range(0, GameManager.Instance.cleanList.Count - 1);
			var bloc = GameManager.Instance.cleanList[randomBloc].transform;

			_lastObj = bloc.gameObject;

			if(_lastObj != null)
				GameManager.Instance.cleanList.Remove(_lastObj);
			
			if (!_isFull)
			{
				var blocPos = bloc.position;
				Instantiate(objectToSpawn, new Vector3(blocPos.x, blocPos.y + 1f, blocPos.z), Quaternion.identity, bloc);
				break;
			}
		} 
		while
		(!GameManager.Instance.cleanList[Random.Range(0, GameManager.Instance.cleanList.Count  - 1)].transform.GetComponent<Node>().isActive);
	}
}

[Serializable]
public class Inventory
{
	public Transform spawnObject;
	public int objectAcquired;
	public List<Image> imageConditions;
	public List<UIWinConditionsTween> uiWinConditionsList;
}