using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DashPower : MonoBehaviour, IManagePower
{
	public int dashRange;

	public List<GameObject> buttons;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private void Start()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].SetActive(true);
		}
	}

	public void ButtonClickedGrab(int numberDirectionVector)
	{
		var position = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = position;

		RaycastHit hit;

		if (Physics.Raycast(transform.position, _vectorRaycast[numberDirectionVector], out hit, dashRange))
		{
			if (hit.collider.gameObject.layer == 3)
			{
				var distance = Vector3.Distance(position, hit.collider.transform.position);
				Debug.Log(distance);
			}
			else if (hit.collider.gameObject.layer == 6)
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(position, hit.collider.transform.position);
				if (distanceBetweenTwoPlayers <= dashRange + 0.1f)
				{
					switch (distanceBetweenTwoPlayers)
					{
						case 1:
							distanceBetweenTwoPlayers = 3;
							break;
						case 3:
							distanceBetweenTwoPlayers = 1;
							break;
					}

					if (Physics.Raycast(hit.transform.position, _vectorRaycast[numberDirectionVector], out var hitPlayerTouched, distanceBetweenTwoPlayers))
					{
						var distanceBetweenBlockAndPlayerTouched = Vector3.Distance(hit.transform.position,
							hitPlayerTouched.transform.position);
						
						var distanceBetweenTwoPlayersWhenABlockIsBehind = Vector3.Distance(position, hit.collider.transform.position);
						distanceBetweenTwoPlayersWhenABlockIsBehind = (int) distanceBetweenTwoPlayersWhenABlockIsBehind;
						
						
						switch (distanceBetweenTwoPlayersWhenABlockIsBehind)
						{
							case 1:
								break;
							case 2:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] * 
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), 0.05f);
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] * 
									(distanceBetweenTwoPlayers -1), 0.05f);
								break;
						}

						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[numberDirectionVector] * (distanceBetweenBlockAndPlayerTouched - 1), 1f);
					}
					else
					{
						GameManager.Instance.currentPlayerTurn.transform.DOMove(
							position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[numberDirectionVector] * distanceBetweenTwoPlayers, 1f);
					}
				}
			}
		}
		else
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
		}
	}

	public void ShowPower()
	{
	}

	public void LaunchPower()
	{
	}
}