using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GrabPower : MonoBehaviour, IManagePower
{
	public int grabRange;


	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	void Start()
	{
		//transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
	}

	public void ButtonClickedGrab(int numberDirectionVector)
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;
		
		RaycastHit hit;
		
		if (Physics.Raycast( transform.position, _vectorRaycast[numberDirectionVector], out hit, grabRange))
		{
			if (hit.collider.gameObject.layer == 3)
			{
				GameManager.Instance.currentPlayerTurn.transform.position = hit.collider.transform.position - _vectorRaycast[numberDirectionVector];
			}
			else if (hit.collider.gameObject.layer == 6)
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(GameManager.Instance.currentPlayerTurn.transform.position + _vectorRaycast[numberDirectionVector], 1f);
				hit.collider.transform.DOMove(hit.collider.transform.position - _vectorRaycast[numberDirectionVector], 1f);
			}
		}
	}
	public void ShowPower()
	{
		
	}

	public void LaunchPower()
	{
	}
}