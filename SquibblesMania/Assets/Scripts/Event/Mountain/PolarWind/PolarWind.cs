using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PolarWind : MonoBehaviour, IManageEvent
{
	[Range(0, 5)] public int hideRaycastDistance;
	[Range(0, 10)] public int turnMinBeforeActivate, turnMaxBeforeActivate;
	[Range(0, 5)] public int distanceMovingPlayer;

	[Range(0.0f, 3.0f)] public float speedPlayer;
	
	public Transform spawnWind;
	
	public LayerMask layerBlocsWhichCanHide;
	
	public TextMeshProUGUI windIsComing;

	private GameObject _windGo;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	private int _turnNumberChosenToLaunchTheWind;
	private int _turnCount;
	private int _directionChosen;
	private bool _isLaunched;
	[HideInInspector] public List<GameObject> hideParticle = new List<GameObject>();
	public GameObject[] particlePlayer = new GameObject[4];

	private const float MoreTimeToCheckUnderPlayer = 0.5f;
	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent() // Show Event add Sound and change the direction of the wind
	{
		AudioManager.Instance.Play("SoftWindLoop");
		
		_turnNumberChosenToLaunchTheWind = Random.Range(turnMinBeforeActivate, turnMaxBeforeActivate);

		windIsComing.gameObject.SetActive(true);
		
		_turnCount = GameManager.Instance.turnCount;

		_directionChosen = Random.Range(0, _vectorRaycast.Count);
		
		_windGo = PoolManager.Instance.SpawnObjectFromPool("ParticleWind", spawnWind.position, Quaternion.identity, null);

		var rot = _windGo.transform.rotation;
		
		switch (_directionChosen)
		{
			case 0 : _windGo.transform.rotation = Quaternion.Euler(rot.x, -90, rot.z);
				break;
			case 1 : _windGo.transform.rotation = Quaternion.Euler(rot.x, 90, rot.z);
				break;
			case 2 : _windGo.transform.rotation = Quaternion.Euler(rot.x, 180, rot.z);
				break;
			case 3 : _windGo.transform.rotation = Quaternion.Euler(rot.x, 0, rot.z);
				break;
		}

		CheckIfPlayersAreHide();
	}

	public void LaunchEvent() // Launch the event, check if player are hide, if they don't, push them away
	{
		if (_turnCount + _turnNumberChosenToLaunchTheWind <= GameManager.Instance.turnCount && GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && !_isLaunched)
		{
			AudioManager.Instance.Play("SoftStrongWind");
			var players = GameManager.Instance.players;

			for (int i = 0; i < players.Count; i++)
			{
				if (Physics.Raycast(players[i].transform.position, -_vectorRaycast[_directionChosen], hideRaycastDistance, layerBlocsWhichCanHide) && !players[i].isPlayerHide)
				{
					var distBetweenBlocAndPlayer = Vector3.Distance(players[i].transform.position, -_vectorRaycast[_directionChosen]);
					distBetweenBlocAndPlayer = (int) distBetweenBlocAndPlayer;

					switch (distBetweenBlocAndPlayer)
					{
						case 0 : break;
						case 1 : players[i].transform.DOMove(players[i].transform.position + ((-_vectorRaycast[_directionChosen])), speedPlayer);
							break;
					}
				}
				else if(!players[i].isPlayerHide)
				{
					players[i].transform.DOMove(players[i].transform.position + -_vectorRaycast[_directionChosen] * distanceMovingPlayer, speedPlayer);
				}
			}
			
			for (int i = 0; i < hideParticle.Count; i++)
			{
				hideParticle[i].SetActive(false);
			}
			
			hideParticle.Clear();

			_isLaunched = true;
			
			StartCoroutine(WaitBeforeCheckUnderPlayer());
			
			_windGo.SetActive(false);
			windIsComing.gameObject.SetActive(false);
		}
	}

	IEnumerator WaitBeforeCheckUnderPlayer() // Detect parent below player
	{
		yield return new WaitForSeconds(speedPlayer + MoreTimeToCheckUnderPlayer);
		
		GameManager.Instance.DetectParentBelowPlayers();
		_isLaunched = false;
		gameObject.SetActive(false);
	}
	
	public void CheckIfPlayersAreHide() // Check if players are hide, every power used or movement made
	{
		if (gameObject.activeSelf)
		{
			var players = GameManager.Instance.players;

			for (int i = 0; i < players.Count; i++)
			{
				if (Physics.Raycast(players[i].transform.position, _vectorRaycast[_directionChosen], hideRaycastDistance, layerBlocsWhichCanHide))
				{
					players[i].isPlayerHide = true;
				}
				else
				{
					players[i].isPlayerHide = false;
				}
				
				if (!players[i].isPlayerHide)
				{
					GameObject vfx = PoolManager.Instance.SpawnObjectFromPool("ParticleWindIndicator", players[i].transform.position + new Vector3(0, 2, 0), Quaternion.identity, players[i].transform);
					hideParticle.Add(vfx);
					particlePlayer[i] = vfx;
				}
				else
				{
					particlePlayer[i] = null;
				}
			}
		}
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