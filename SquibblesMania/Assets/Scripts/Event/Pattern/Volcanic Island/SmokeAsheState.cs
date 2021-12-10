using System.Collections.Generic;
using UnityEngine;

public class SmokeAsheState : EventBaseState
{
	[Header("EVENT")]
	private List<GameObject> _cubeOnMap;

	private int _numberOfMeteorite;
	public override void EnterState(EventStateManager eventState)
	{
		_cubeOnMap = MapGenerator.Instance.cubeOnMap;
		_numberOfMeteorite = MapGenerator.Instance.numberOfMeteorite;
		
		int heightOfMap = 10;
		int cubeToChange = Random.Range(0, 80);

		for (int i = 0; i <= _numberOfMeteorite; i++)
		{
			_cubeOnMap[cubeToChange + i].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[cubeToChange+ i +heightOfMap].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[cubeToChange+ i +heightOfMap*2].GetComponent<Renderer>().material.color = Color.black;
		}
	}

	public override void UpdateState(EventStateManager eventState)
	{
		
	}

	public override void OnCollisionEnter(EventStateManager eventState)
	{
		
	}
}
