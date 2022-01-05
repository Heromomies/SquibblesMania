using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEvent : MonoBehaviour
{
	[Header("EVENT")] private List<GameObject> _cubeOnMap;
	private List<GameObject> _cubeTouched;
	public GameObject eventParticle;

	public int numberOfSmoke;
	public float heightSpawnParticle;
	private int _cubeToChange;
	
	public void Start() // On click we launch the compact function
	{
		#region LaunchFunctionCompactEvent

		_cubeOnMap = EventManager.Instance.cubeOnMap;

		CompactEvent();

		#endregion
	}

	private void CompactEvent() // Compact event is equal to the smoke event
	{
		int heightOfMap = 10; 
		CubeToChange();

		for (int i = 0; i <= numberOfSmoke; i++) // Allow to colorize material to create a compact zone
		{
			_cubeOnMap[_cubeToChange + i].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[_cubeToChange + i + heightOfMap].GetComponent<Renderer>().material.color = Color.black;
			_cubeOnMap[_cubeToChange + i + heightOfMap * 2].GetComponent<Renderer>().material.color = Color.black;

			_cubeTouched.Add(_cubeOnMap[_cubeToChange + i]);
			_cubeTouched.Add(_cubeOnMap[_cubeToChange + i + heightOfMap]);
			_cubeTouched.Add(_cubeOnMap[_cubeToChange + i + heightOfMap*2]);
		} 
		foreach (var cubeTouched in _cubeTouched) // Instantiate particle on the top of the block
		{
			float y = cubeTouched.transform.localScale.y;
			
			Instantiate(eventParticle, new Vector3(cubeTouched.transform.position.x, y+heightSpawnParticle, cubeTouched.transform.position.z), Quaternion.identity);
		}
	}

	void CubeToChange() // Change the value, now the smoke event can be divided by 2
	{
		_cubeToChange = Random.Range(0, _cubeTouched.Count);
		
		while (_cubeToChange >= 6 && _cubeToChange <= 10 || _cubeToChange >= 14 && _cubeToChange <= 20 || _cubeToChange >= 24 && _cubeToChange <= 30
		    || _cubeToChange >= 34 && _cubeToChange <= 40 || _cubeToChange >= 44 && _cubeToChange <= 50 || _cubeToChange >= 54 && _cubeToChange <= 60
		    || _cubeToChange >= 64 && _cubeToChange <= 70 || _cubeToChange >= 74 && _cubeToChange <= 80)
		{
			_cubeToChange = Random.Range(0, _cubeTouched.Count);
		}
	}
}