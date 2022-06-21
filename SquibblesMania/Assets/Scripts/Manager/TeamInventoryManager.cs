using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
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

	[Header("CONTROL ALEATORY")]
	[Range(0,10)] public float radiusMin;
	[Range(0,10)] public float radiusMax;
	public LayerMask layerInteractable;
	public List<GameObject> colliderFinished;
	public List<GameObject> colliderFinishedMax;
	public ColliderStruct[] colliderStructMax;
	
	[System.Serializable]
	public struct ColliderStruct
	{
		public Collider[] Collider;
	}

	[Header("SCALE ICON IMG TRANSPORT")]
	[SerializeField] private Vector3 scaleSizeTween = new Vector3(1.3f, 1.3f, 1.3f);
	[SerializeField] private float timeInSecondsScaleTween = 0.5f;
	[SerializeField] private LeanTweenType easeScaleType;


	private static float _timeBeforeSpawnItems = 0.7f;
	private WaitForSeconds _waitForSecondsSpawnItems = new WaitForSeconds(_timeBeforeSpawnItems);
	private	void Awake()
	{
		_teamInventoryManager = this;
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.2f);

		colliderFinishedMax = GameManager.Instance.cleanList;
		
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

		if (!_isFull)
		{
			var players = GameManager.Instance.players;
			colliderFinished = new List<GameObject>(colliderFinishedMax);

			if (inventory[1].objectAcquired < inventory[0].objectAcquired) // Team 1 is before Team 2
			{
				// ReSharper disable once Unity.PreferNonAllocApi
				colliderStructMax[0].Collider = Physics.OverlapSphere(players[0].transform.position, radiusMax, layerInteractable);
				// ReSharper disable once Unity.PreferNonAllocApi
				colliderStructMax[2].Collider = Physics.OverlapSphere(players[2].transform.position, radiusMax, layerInteractable);
				
				// ReSharper disable once Unity.PreferNonAllocApi
				colliderStructMax[1].Collider = Physics.OverlapSphere(players[1].transform.position, radiusMin, layerInteractable);
				// ReSharper disable once Unity.PreferNonAllocApi
				colliderStructMax[3].Collider = Physics.OverlapSphere(players[3].transform.position, radiusMin, layerInteractable);

				for (int i = 0; i < players.Count; i++)
				{
					for (int j = 0; j < colliderStructMax[i].Collider.Length; j++)
					{
						if (colliderFinished.Contains(colliderStructMax[i].Collider[j].gameObject))
						{
							colliderFinished.Remove(colliderStructMax[i].Collider[j].gameObject);
						}
					}
				}
			}
			else if (inventory[1].objectAcquired > inventory[0].objectAcquired) // Team 2 is before Team 1
			{
				for (int i = 0; i < players.Count; i++)
				{
					// ReSharper disable once Unity.PreferNonAllocApi
					colliderStructMax[i].Collider = Physics.OverlapSphere(players[i].transform.position, radiusMax, layerInteractable);
					
					for (int j = 0; j < colliderStructMax[i].Collider.Length; j++)
					{
						if (colliderFinished.Contains(colliderStructMax[i].Collider[j].gameObject))
						{
							colliderFinished.Remove(colliderStructMax[i].Collider[j].gameObject);
						}
					}
				}
			}
			else if (inventory[0].objectAcquired == inventory[1].objectAcquired) // Team 2 is equal to Team 1
			{
				colliderFinished = new List<GameObject>(GameManager.Instance.cleanList);
			}
			
			if (colliderFinished.Count > 0)
			{
				for (int i = 0; i < colliderFinished.Count; i++)
				{
					if (!colliderFinished[i].GetComponent<Node>().isActive)
					{
						colliderFinished.Remove(colliderFinished[i]);
					}
				}

				StartCoroutine(SpawnObject());
			}
			else
			{
				colliderFinished =  new List<GameObject>(colliderFinishedMax);
				
				for (int i = 0; i < colliderFinished.Count; i++)
				{
					if (!colliderFinished[i].GetComponent<Node>().isActive)
					{
						colliderFinished.Remove(colliderFinished[i]);
					}
				}
				
				StartCoroutine(SpawnObject());
			}

			colliderFinishedMax = GameManager.Instance.cleanList;
		}
	}

	IEnumerator SpawnObject()
	{
		yield return _waitForSecondsSpawnItems;
		
		var randomBloc = Random.Range(0, colliderFinished.Count);
		var bloc = colliderFinished[randomBloc].transform;
		
		var blocPos = bloc.position;
				
		Instantiate(objectToSpawn, new Vector3(blocPos.x, blocPos.y + 1f, blocPos.z), Quaternion.identity, bloc);
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