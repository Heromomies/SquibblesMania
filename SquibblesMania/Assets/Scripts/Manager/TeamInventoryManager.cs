using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TeamInventoryManager : MonoBehaviour
{
	[Header("Balloon")] public Inventory[] inventory;
	public List<Transform> objectTransport;
	public GameObject objectToSpawn;

	private bool _isFull;
	[SerializeField] private int maxItemNumberAcquired = 3;
	private static TeamInventoryManager _teamInventoryManager;

	public static TeamInventoryManager Instance => _teamInventoryManager;

	[Header("Objective Display")]
	private GameObject _lastObj;
	
	[Header("SCALE ICON IMG TRANSPORT")]
	[SerializeField] private Vector3 scaleSizeTween = new Vector3(1.3f, 1.3f, 1.3f);
	[SerializeField] private float timeInSecondsScaleTween = 0.5f;
	[SerializeField] private LeanTweenType easeScaleType;
	
	private	void Awake()
	{
		_teamInventoryManager = this;
	}

	private void Start()
	 {
		 inventory[0].balloonTransformParent = Instantiate(objectTransport[3], inventory[0].spawnObject.position, inventory[0].spawnObject.rotation, inventory[0].spawnObject);
		 inventory[1].balloonTransformParent = Instantiate(objectTransport[4], inventory[1].spawnObject.position, inventory[1].spawnObject.rotation, inventory[1].spawnObject);
	 }
	

	public void AddResourcesToInventory(int indexObject, Player.PlayerTeam playerTeam) // Add resources to the inventory
	{
		if (playerTeam == Player.PlayerTeam.TeamOne && inventory[0].objectAcquired < maxItemNumberAcquired)
		{
			Instantiate(objectTransport[inventory[0].objectAcquired], inventory[0].spawnObject.position, inventory[0].spawnObject.transform.rotation, inventory[0].balloonTransformParent);
			inventory[0].uiWinConditionsList[inventory[0].objectAcquired].gameObject.SetActive(true);
			inventory[0].objectAcquired += indexObject;
			CheckPlayerTotalItemAcquired(inventory[0]);
			Destroy(inventory[0].balloonTransformParent.gameObject.transform.GetChild(0).gameObject);
		}

		if (playerTeam == Player.PlayerTeam.TeamTwo && inventory[1].objectAcquired < maxItemNumberAcquired)
		{
			//Add Offset to the ballon rotation
			Quaternion rotOffset  = Quaternion.Euler(0,180f,0);
			Instantiate(objectTransport[inventory[1].objectAcquired], inventory[1].spawnObject.position, inventory[1].spawnObject.transform.rotation * rotOffset, inventory[1].balloonTransformParent);
			inventory[1].uiWinConditionsList[inventory[1].objectAcquired].gameObject.SetActive(true);
			inventory[1].objectAcquired += indexObject;
			CheckPlayerTotalItemAcquired(inventory[1]);
			Destroy(inventory[1].balloonTransformParent.gameObject.transform.GetChild(0).gameObject);
		}

		if (inventory[0].objectAcquired == maxItemNumberAcquired || inventory[1].objectAcquired == maxItemNumberAcquired)
		{
			GameManager.Instance.isConditionVictory = true;
			GameManager.Instance.ShowEndZone();
		}

		if (inventory[0].objectAcquired == maxItemNumberAcquired && inventory[1].objectAcquired == maxItemNumberAcquired)
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

	private void CheckPlayerTotalItemAcquired(Inventory playerInventory)
	{
		if (playerInventory.objectAcquired == maxItemNumberAcquired)
		{
			TweenScaleImgTransport(playerInventory.uiImgTransport);
			foreach (var uiWinConditions in playerInventory.uiWinConditionsList)
			{
				uiWinConditions.endZoneItemColor = Color.yellow;
			}
		}
	}
	
	/// <summary>
	/// Scale anim using leantween 
	/// </summary>
	/// <param name="uiImgTransport"></param>
	private void TweenScaleImgTransport( RectTransform uiImgTransport)
	{
		LeanTween.scale(uiImgTransport, scaleSizeTween, timeInSecondsScaleTween).setEase(easeScaleType)
			.setLoopPingPong();
	}
}



[Serializable]
public class Inventory
{
	public Transform balloonTransformParent;
	public Transform spawnObject;
	public int objectAcquired;
	public RectTransform uiImgTransport;
	public List<UIWinConditionsTween> uiWinConditionsList;
}