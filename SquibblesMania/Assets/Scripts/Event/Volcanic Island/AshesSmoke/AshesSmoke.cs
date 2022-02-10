using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshesSmoke : MonoBehaviour, IManageEvent
{
	[Header("EVENT")] private List<GameObject> _cubeOnMap;
	[HideInInspector] public List<GameObject> cubeTouched;
	public GameObject eventParticle;

	public int numberOfSmoke;
	public float heightSpawnParticle;
	private int _cubeToChange;
	
	public void ShowEvent() // Show the event
	{
		_cubeOnMap = EventManager.Instance.cleanList;
		Debug.Log(_cubeOnMap.Count);
		
		_cubeToChange = Random.Range(0, _cubeOnMap.Count - numberOfSmoke);

		for (int i = 0; i < numberOfSmoke; i++) // Allow to colorize material to create a compact zone
		{
			_cubeOnMap[_cubeToChange + i].GetComponent<Renderer>().material.color = Color.black;
			cubeTouched.Add(_cubeOnMap[_cubeToChange + i]);
		} 
	}

	public void LaunchEvent() // Launch the event
	{
		foreach (var cubeTouched in cubeTouched) // Instantiate particle on the top of the block
		{
			float y = cubeTouched.transform.localScale.y;
			
			Instantiate(eventParticle, new Vector3(cubeTouched.transform.position.x, y+heightSpawnParticle, cubeTouched.transform.position.z), Quaternion.identity);
		}
	}
}