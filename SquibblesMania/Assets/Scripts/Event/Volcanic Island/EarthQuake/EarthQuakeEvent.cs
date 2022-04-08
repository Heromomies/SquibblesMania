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
	
	[Space]
	[Header("CONDITIONS DANGEROUSNESS")]
	
	public Conditions[] conditionsDangerousnessEarthQuake;

	[Serializable]
	public struct Conditions
	{
		public int numberOfBlocsTouched;
	}

	private float _playerHeight = 2.5f;

	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent()
	{
		var blocParent = GameManager.Instance.allBlocks;
		
		AudioManager.Instance.Play("VolcanoShaking");
		
		for (int i = 0; i < conditionsDangerousnessEarthQuake[EventManager.Instance.dangerousness].numberOfBlocsTouched; i++) // Set the position of random blocs touched in Y equal to 0
		{
			int randomNumber = Random.Range(0, blocParent.Count);
			if (Math.Abs(blocParent[randomNumber].transform.position.y - 1) < GameManager.Instance.minHeightBlocMovement) // If the Y position is equal to 0, add one bloc to touch
			{
				i--;
			}
			else // When a bloc can be moved 
			{
				var blocHeight = Random.Range(GameManager.Instance.minHeightBlocMovement, GameManager.Instance.maxHeightBlocMovement);
				
				var col = blocParent[randomNumber].transform.position;

				var blocParentPlayer = blocParent[randomNumber].GetComponent<GroupBlockDetection>().playersOnGroupBlock;
				
				if (Math.Abs(col.y - blocHeight) > 0.1f)
				{
					col = new Vector3(col.x, blocHeight, col.z);
					blocParent[randomNumber].transform.DOMove(col, speedBloc);
					
					for (int j = 0; j < blocParentPlayer.Count; j++)
					{				
						var blocParentPlayerPos = blocParent[randomNumber].GetComponent<GroupBlockDetection>().playersOnGroupBlock[j].transform.position;

						if (blocParent[randomNumber].transform.position.y < blocHeight)
						{
							blocParentPlayerPos = new Vector3(blocParentPlayerPos.x, blocHeight + _playerHeight, blocParentPlayerPos.z);
							blocParentPlayer[j].transform.DOMove(blocParentPlayerPos, speedBloc); 
						}
					}
				}
				else
				{
					i--;
				}
				
				
			}
		}
		
		StartCoroutine(SetActiveFalseBullet());
	}
	IEnumerator SetActiveFalseBullet()
	{
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}
	public void LaunchEvent()
	{
	}
}