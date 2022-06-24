using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EarthQuakeEvent : MonoBehaviour, IManageEvent
{
	[Header("EVENT SETTINGS")]
	[Range(0f, 10f)] public float speedBloc;
	
	private static float _timeBeforeSetActiveFalseLava = 1f;
	private WaitForSeconds _waitTimeBeforeSetActiveFalseLaval = new WaitForSeconds(_timeBeforeSetActiveFalseLava);
	private List<Transform> _blocParentPlayer;
	private float _posPlayerToAdd = 1.5f;
	[Space]
	[Header("CONDITIONS DANGEROUSNESS")]
	
	public Conditions[] conditionsDangerousnessEarthQuake;

	[Serializable]
	public struct Conditions
	{
		public int numberOfBlocsTouched;
	}
	
	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent()
	{
		var blocParent = GameManager.Instance.allBlocParents;
		
		AudioManager.Instance.Play("VolcanoShaking");
		
		for (int i = 0; i < conditionsDangerousnessEarthQuake[VolcanoManager.Instance.dangerousness].numberOfBlocsTouched; i++) // Set the position of random blocs touched in Y equal to 0
		{
			int randomNumber = Random.Range(0, blocParent.Count);
			if (Math.Abs(blocParent[randomNumber].transform.position.y - 1) < GameManager.Instance.minHeightBlocMovement) // If the Y position is equal to 0, add one bloc to touch
			{
				i--;
			}
			else // When a bloc can be moved 
			{
				var blocHeight = Random.Range(GameManager.Instance.minHeightBlocMovement, GameManager.Instance.maxHeightBlocMovement);
				
				var blocParentPos = blocParent[randomNumber].transform.position;

				if (blocParent[randomNumber].TryGetComponent(out GroupBlockDetection groupBlockDetection))
				{
					_blocParentPlayer = groupBlockDetection.playersOnGroupBlock;
				}

				if (Math.Abs(blocParentPos.y - blocHeight) > 0.1f)
				{
					blocParentPos = new Vector3(blocParentPos.x, blocHeight, blocParentPos.z);
					blocParent[randomNumber].transform.DOMove(blocParentPos, speedBloc);
					
					for (int j = 0; j < _blocParentPlayer.Count; j++)
					{				
						var blocParentPlayerPos = _blocParentPlayer[j].transform.position;

						if (blocParent[randomNumber].transform.position.y < blocHeight)
						{
							blocParentPlayerPos = new Vector3(blocParentPlayerPos.x, blocParentPos.y +_posPlayerToAdd, blocParentPlayerPos.z);
							_blocParentPlayer[j].transform.DOMove(blocParentPlayerPos, speedBloc); 
						}
					}
				}
				else
				{
					i--;
				}
				
				
			}
		}
		
		GameManager.Instance.DetectParentBelowPlayers();
		
		StartCoroutine(SetActiveFalseBullet());
	}
	IEnumerator SetActiveFalseBullet()
	{
		yield return _waitTimeBeforeSetActiveFalseLaval;
		
		gameObject.SetActive(false);
	}
	public void LaunchEvent()
	{
	}
}