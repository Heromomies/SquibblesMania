using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EarthQuakeEvent : MonoBehaviour, IManageEvent
{
	public int radius;
	public LayerMask layer;
	public Collider[] colliders;
	
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
		colliders = Physics.OverlapSphere(gameObject.transform.position, radius, layer);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].transform.position.y == 0)
			{
				colliders.ToList().Remove(colliders[i]);
			}
		}
		for (int j = 0; j < conditionsDangerousnessEarthQuake[EventManager.Instance.dangerousness].numberOfBlocsTouched; j++)
		{
			Debug.Log(colliders.Length);
			int randomNumber = Random.Range(0, colliders.Length);
			var col = colliders[randomNumber].transform.position;
					
			col = new Vector3(col.x, 0, col.z);
			colliders[randomNumber].transform.position = col;
		}
		
		gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
		Gizmos.color = Color.magenta;
	}

	public void LaunchEvent()
	{
	}
}