using System;
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
	public Collider[] colliderFinished;
	public ColliderStruct[] colliderStructMax;
	
	[System.Serializable]
	public struct ColliderStruct
	{
		public Collider[] Collider;
	}
	
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

		if (!_isFull)
		{
			var players = GameManager.Instance.players;
		
			for (int i = 0; i <  players.Count; i++)
			{
				var a = GetDonut(players[i].transform.position, radiusMin, radiusMax, layerInteractable);

				colliderStructMax[i].Collider = a.ToArray();
			}
			
			var firstColList = new List<Collider>();
			var firstColArray = firstColList.ToArray();
		
			var secondColList = new List<Collider>();
			var secondColArray = firstColList.ToArray();
			
			var finalList = new List<Collider>();
			var finalArray = finalList.ToArray();
			
			if (inventory[1].objectAcquired < inventory[0].objectAcquired) // Team 1 is before Team 2
			{
				firstColArray = colliderStructMax[0].Collider.Concat(colliderStructMax[2].Collider).ToArray(); // Collider Team 1
				secondColArray = colliderStructMax[1].Collider.Concat(colliderStructMax[3].Collider).ToArray(); // Collider Team 2
				
				firstColList = firstColArray.ToList();
				secondColList = secondColArray.ToList();

				for (int i = 0; i < firstColList.Count; i++)
				{
					if (secondColList.Contains(firstColList[i]))
					{
						secondColList.Remove(firstColList[i]);
					}
				}

				secondColArray = secondColList.ToArray();
				finalArray = secondColArray;
				finalList = finalArray.ToList();

				for (int i = 0; i < finalList.Count; i++)
				{
					if (finalList[i].TryGetComponent(out Node node))
					{
						if (!node.isActive)
						{
							finalList.Remove(finalList[i]);
						}
					}
				}
				
				finalArray = finalList.ToArray();
				colliderFinished = finalArray;
			}
			else if (inventory[0].objectAcquired < inventory[1].objectAcquired) // Team 2 is before Team 1
			{
				firstColArray = colliderStructMax[0].Collider.Concat(colliderStructMax[2].Collider).ToArray(); // Collider Team 1
				secondColArray = colliderStructMax[1].Collider.Concat(colliderStructMax[3].Collider).ToArray(); // Collider Team 2

				firstColList = firstColArray.ToList();
				secondColList = secondColArray.ToList();
				foreach (var coll in secondColList)
				{
					if (firstColList.Contains(coll))
					{
						firstColList.Remove(coll);
					}
				}
				
				firstColArray = firstColList.ToArray();
				finalArray = firstColArray;
				finalList = finalArray.ToList();
				
				
				for (int i = 0; i < finalList.Count; i++)
				{
					if (finalList[i].TryGetComponent(out Node node))
					{
						if (!node.isActive)
						{
							finalList.Remove(finalList[i]);
						}
					}
				}
				
				finalArray = finalList.ToArray();
				colliderFinished = finalArray;
			}
			else if (inventory[0].objectAcquired == inventory[1].objectAcquired) // Team 2 is equal to Team 1
			{
				firstColArray = colliderStructMax[0].Collider.Concat(colliderStructMax[2].Collider).ToArray(); // Collider Team 1
				secondColArray = colliderStructMax[1].Collider.Concat(colliderStructMax[3].Collider).ToArray(); // Collider Team 2
				finalArray = firstColArray.Concat(secondColArray).ToArray();
				
				finalList = finalArray.ToList();
				
				for (int i = 0; i < finalList.Count; i++)
				{
					if (finalList[i].TryGetComponent(out Node node))
					{
						if (!node.isActive)
						{
							finalList.Remove(finalList[i]);
						}
					}
				}

				colliderFinished = finalList.ToArray();
			}
			
			var randomBloc = Random.Range(0, colliderFinished.Length);
			var bloc = colliderFinished[randomBloc].transform;
				
			var blocPos = bloc.position;

			Instantiate(objectToSpawn, new Vector3(blocPos.x, blocPos.y + 1f, blocPos.z), Quaternion.identity, bloc);
		}
		
	}
	
	public static List<Collider> GetDonut(Vector3 pos, float innerRadius, float outerRadius, LayerMask layer)
	{
		List<Collider> outer = new List<Collider>(Physics.OverlapSphere(pos,outerRadius, layer));
		Collider[] inner = Physics.OverlapSphere(pos,innerRadius, layer);
		foreach (Collider C in inner)
			outer.Remove(C);
		return outer;
	}
	
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var p = GameManager.Instance.players;
		Gizmos.color = Color.red;
		for (int i = 0; i < p.Count; i++)
		{
			Gizmos.DrawWireSphere(p[i].transform.position, radiusMin);
		}
		
		Gizmos.color = Color.blue;
		for (int i = 0; i < p.Count; i++)
		{
			Gizmos.DrawWireSphere(p[i].transform.position, radiusMax);
		}
	}
#endif
	
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