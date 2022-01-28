using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DashPower : MonoBehaviour, IManagePower
{
	public int dashRange;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

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
						case 1: distanceBetweenTwoPlayers = 3;
							break;
						case 3: distanceBetweenTwoPlayers = 1;
							break;
					}
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
					hit.collider.transform.DOMove(hit.collider.transform.position
					                              + _vectorRaycast[numberDirectionVector] * distanceBetweenTwoPlayers, 1f);
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