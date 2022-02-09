using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DashPower : MonoBehaviour, IManagePower
{
	public int dashRange;

	public List<GameObject> buttons;
	public LayerMask layerMask;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private void OnEnable()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].SetActive(true);
		}
	}

	public void ButtonClickedDash(int numberDirectionVector) // When we clicked on button
	{
		var position = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = position;

		if (Physics.Raycast(transform.position, _vectorRaycast[numberDirectionVector], out var hit, dashRange)) // launch the raycast
		{
			if (hit.collider.gameObject.layer == 3)
			{
				var distance = Vector3.Distance(position, hit.collider.transform.position);

				GameManager.Instance.currentPlayerTurn.transform.DOMove(
					position + _vectorRaycast[numberDirectionVector] * (distance - 1), 0.05f);
			}
			else if (hit.collider.gameObject.layer == 6) // When the raycast touch another player
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(position, hit.collider.transform.position);
				distanceBetweenTwoPlayers += 0.1f;
				distanceBetweenTwoPlayers = (int) distanceBetweenTwoPlayers; // check distance between two players

				switch (distanceBetweenTwoPlayers) // inverse distance for the dash, else the player repulsed don't follow the range  
				{
					case 1:
						distanceBetweenTwoPlayers = 3;
						break;
					case 3:
						distanceBetweenTwoPlayers = 1;
						break;
				}

				if (Physics.Raycast(hit.transform.position, _vectorRaycast[numberDirectionVector], out var hitPlayerTouched, distanceBetweenTwoPlayers,
					layerMask)) // If the player repulsed touch a block behind him
				{
					var distanceBetweenBlockAndPlayerTouched = Vector3.Distance(hit.transform.position,
						hitPlayerTouched.transform.position);
					distanceBetweenBlockAndPlayerTouched += 0.1f;
					distanceBetweenBlockAndPlayerTouched = (int) distanceBetweenBlockAndPlayerTouched; //Check distance between himself and the block behind him

					var distanceBetweenTwoPlayersWhenABlockIsBehind = Vector3.Distance(position, hit.collider.transform.position);
					distanceBetweenTwoPlayersWhenABlockIsBehind += 0.1f;
					distanceBetweenTwoPlayersWhenABlockIsBehind =
						(int) distanceBetweenTwoPlayersWhenABlockIsBehind; // Check the distance between the two players

					if (distanceBetweenBlockAndPlayerTouched > 1)
					{
						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // inverse distance for the dash, else the player repulsed don't follow the range  
						{
							case 1:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 3;
								break;
							case 3:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 1;
								break;
						}

						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // according to the distance between the two players, the dash is not the same
						{
							case 2:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), 0.05f);
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] *
									(distanceBetweenTwoPlayers - 1), 0.05f);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[numberDirectionVector] * (distanceBetweenBlockAndPlayerTouched - 1), 1f);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
					hit.collider.transform.DOMove(hit.collider.transform.position
					                              + _vectorRaycast[numberDirectionVector] * distanceBetweenTwoPlayers, 1f);
				}
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
		}

		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
		foreach (var button in buttons)
		{
			button.SetActive(false);
		}
	}

	public void ShowPower()
	{
	}

	public void LaunchPower()
	{
	}
}