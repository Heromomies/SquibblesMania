using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EruptionState : EventBaseState
{
	[Header("EVENT")]
	private List<GameObject> _cubeOnMap;

	private int _numberOfMeteorite;

	public GameObject go;
	public override void EnterState(EventStateManager eventState)
	{
		#region ExplosionFromTheCenter

		
		for (int i = 0; i < _numberOfMeteorite; i++)
		{	
			
		}

		#endregion


		#region MeteoriteRandomization

		_cubeOnMap = MapGenerator.Instance.cubeOnMap;
		_numberOfMeteorite = MapGenerator.Instance.numberOfMeteorite;
		
		for (int i = 0; i <= _numberOfMeteorite; i++)
		{
			int placeOfCube = Random.Range(0, 100);
			RandomEvent(placeOfCube);
		}

		#endregion
	}

	private void RandomEvent(int i)
	{
		if (_cubeOnMap[i].GetComponent<Renderer>().material.color != Color.black)
		{
			_cubeOnMap[i].GetComponent<Renderer>().material.color = Color.black;
			MapGenerator.Instance.cubeOnMap.Remove(_cubeOnMap[i]);
		}
	}
	public override void UpdateState(EventStateManager eventState)
	{
		
	}

	public override void OnCollisionEnter(EventStateManager eventState)
	{
		
	}
}
