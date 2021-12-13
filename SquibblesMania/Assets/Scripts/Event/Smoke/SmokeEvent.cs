using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEvent : MonoBehaviour
{
	[Header("EVENT")] private List<GameObject> _cubeOnMap;
	public GameObject eventParticle;

	public int numberOfSmoke;
	public int heightSpawnParticle;

	public void OnClick()
	{
		#region MeteoriteRandomization

		_cubeOnMap = MapGenerator.Instance.cubeOnMap;

		CompactEvent();

		#endregion
	}

	public void CompactEvent()
	{
		int heightOfMap = 10;
		int cubeToChange = Random.Range(0, 80);

		for (int i = 0; i <= numberOfSmoke; i++)
		{
			_cubeOnMap[cubeToChange + i].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[cubeToChange + i + heightOfMap].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[cubeToChange + i + heightOfMap * 2].GetComponent<Renderer>().material.color = Color.black;

			Instantiate(eventParticle,
				new Vector3(_cubeOnMap[cubeToChange + i].transform.position.x, _cubeOnMap[cubeToChange + i].transform.position.y + heightSpawnParticle,
					_cubeOnMap[cubeToChange + i].transform.position.z), Quaternion.identity);
			Instantiate(eventParticle,
				new Vector3(_cubeOnMap[cubeToChange + i + heightOfMap].transform.position.x,
					_cubeOnMap[cubeToChange + i].transform.position.y + heightSpawnParticle, _cubeOnMap[cubeToChange + i].transform.position.z),
				Quaternion.identity);
			Instantiate(eventParticle,
				new Vector3(_cubeOnMap[cubeToChange + i + heightOfMap * 2].transform.position.x,
					_cubeOnMap[cubeToChange + i].transform.position.y + heightSpawnParticle, _cubeOnMap[cubeToChange + i].transform.position.z),
				Quaternion.identity);
		}
	}
}