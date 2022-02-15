using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GrabPower : MonoBehaviour
{
	public int grabRange;

	public List<GameObject> buttons;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	void OnEnable()
	{
		switch (GameManager.Instance.actualCamPreset.presetNumber)
		{
			case 1:
				buttons[0].SetActive(true);
				break;
			case 2 :
				buttons[0].SetActive(true);
				break;
			case 3:
				buttons[1].SetActive(true);
				break;
			case 4 :
				buttons[1].SetActive(true);
				break;
		}
	}

	public void ButtonClickedGrab(int numberDirectionVector) // When we clicked on the button
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
		
		RaycastHit hit;
		
		if (Physics.Raycast( transform.position, _vectorRaycast[numberDirectionVector], out hit, grabRange)) // Launch a raycast
		{
			if (hit.collider.gameObject.layer == 3) // if the raycast touch a bloc
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(hit.collider.transform.position - _vectorRaycast[numberDirectionVector], 1f);
			}
			else if (hit.collider.gameObject.layer == 6) // if the raycast touch a player
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(GameManager.Instance.currentPlayerTurn.transform.position, hit.transform.position);
				distanceBetweenTwoPlayers += 0.1f;
				distanceBetweenTwoPlayers = (int) distanceBetweenTwoPlayers;

				switch (distanceBetweenTwoPlayers) // In function to the distance move to certain distance
				{
					case 1 : Debug.Log("Only one bloc separate us");
						break;
					case 2 : hit.collider.transform.DOMove(hit.collider.transform.position - _vectorRaycast[numberDirectionVector], 1f);;
						break;
					case 3 : GameManager.Instance.currentPlayerTurn.transform.DOMove(GameManager.Instance.currentPlayerTurn.transform.position + _vectorRaycast[numberDirectionVector], 1f);
						hit.collider.transform.DOMove(hit.collider.transform.position - _vectorRaycast[numberDirectionVector], 1f);;
						break;
					case 4 :GameManager.Instance.currentPlayerTurn.transform.DOMove(GameManager.Instance.currentPlayerTurn.transform.position + _vectorRaycast[numberDirectionVector], 1f);
						hit.collider.transform.DOMove(hit.collider.transform.position - _vectorRaycast[numberDirectionVector] * 2, 1f);;
						break;
				}
			}
		}
		
		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
		foreach (var button in buttons)
		{
			if (button.activeSelf)
			{
				button.SetActive(false);
			}
		}
	}
}