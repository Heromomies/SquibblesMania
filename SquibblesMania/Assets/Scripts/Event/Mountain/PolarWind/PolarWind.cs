using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor.Recorder.Input;
using UnityEngine;
using Random = UnityEngine.Random;

public class PolarWind : MonoBehaviour, IManageEvent
{
	public int hideRaycastDistance;
	public int turnMinBeforeActivate, turnMaxBeforeActivate;

	public int distanceMovingPlayer;
	
	public LayerMask layerBlocsWhichCanHide;
	
	public TextMeshProUGUI windIsComing;

	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	private int _turnNumberChosenToLaunchTheWind;
	private int _turnCount;
	private int _directionChosen;


	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent()
	{
		_turnNumberChosenToLaunchTheWind = Random.Range(turnMinBeforeActivate, turnMaxBeforeActivate);

		windIsComing.gameObject.SetActive(true);
		_turnCount = GameManager.Instance.turnCount;

		_directionChosen = Random.Range(0, _vectorRaycast.Count);
	}

	public void LaunchEvent()
	{
		if (_turnCount + _turnNumberChosenToLaunchTheWind <= GameManager.Instance.turnCount && GameManager.Instance.currentPlayerTurn.playerActionPoint == 0)
		{
			var players = GameManager.Instance.players;

			for (int i = 0; i < players.Count; i++)
			{
				Debug.Log("Player name : "+ players[i].name + " : Player is hide ? : "+ players[i].isPlayerHide + " : Direction : " + players[i].transform.position);
				
				if (Physics.Raycast(players[i].transform.position, -_vectorRaycast[_directionChosen], distanceMovingPlayer, layerBlocsWhichCanHide) && !players[i].isPlayerHide)
				{
					var distBetweenBlocAndPlayer = Vector3.Distance(players[i].transform.position, -_vectorRaycast[_directionChosen]);
					distBetweenBlocAndPlayer = (int) distBetweenBlocAndPlayer;

					players[i].transform.DOMove(players[i].transform.position + ((-_vectorRaycast[_directionChosen]) * distBetweenBlocAndPlayer), 0.05f);
				}
				else if(!players[i].isPlayerHide)
				{
					players[i].transform.DOMove(players[i].transform.position + -_vectorRaycast[_directionChosen] * distanceMovingPlayer, 0.05f);
				}
			}
			
			windIsComing.gameObject.SetActive(false);
		}
	}
	
	public void CheckIfPlayersAreHide()
	{
		var players = GameManager.Instance.players;

		for (int i = 0; i < players.Count; i++)
		{
			if (Physics.Raycast(players[i].transform.position, _vectorRaycast[_directionChosen], hideRaycastDistance, layerBlocsWhichCanHide) && !players[i].isPlayerHide)
			{
				PoolManager.Instance.SpawnObjectFromPool("StunVFX", players[i].transform.position + new Vector3(0, 1, 0), Quaternion.identity, players[i].transform);

				players[i].isPlayerHide = true;
			}
		}
		
		LaunchEvent();
	}

#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach (var p in GameManager.Instance.players)
		{
			Gizmos.DrawRay(p.transform.position , _vectorRaycast[_directionChosen]);
		}
	}
#endif
}