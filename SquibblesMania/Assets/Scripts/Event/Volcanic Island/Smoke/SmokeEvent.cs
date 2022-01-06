using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEvent : MonoBehaviour
{
	[Header("EVENT")] private List<GameObject> _cubeOnMap;
	public List<GameObject> _cubeTouched;
	public GameObject eventParticle;

	public int numberOfSmoke;
	public float heightSpawnParticle;
	private int _cubeToChange;
	
	public void Start() // On click we launch the compact function
	{
		#region LaunchFunctionCompactEvent

		_cubeOnMap = EventManager.Instance.cleanList;

		CompactEvent();

		#endregion
	}

	private void CompactEvent() // Compact event is equal to the smoke event
	{
		_cubeToChange = Random.Range(0, _cubeOnMap.Count);

		for (int i = 0; i < numberOfSmoke; i++) // Allow to colorize material to create a compact zone
		{
			_cubeOnMap[_cubeToChange + i].GetComponent<Renderer>().material.color = Color.black;

			_cubeTouched.Add(_cubeOnMap[_cubeToChange + i]);
		} 
		foreach (var cubeTouched in _cubeTouched) // Instantiate particle on the top of the block
		{
			float y = cubeTouched.transform.localScale.y;
			
			Instantiate(eventParticle, new Vector3(cubeTouched.transform.position.x, y+heightSpawnParticle, cubeTouched.transform.position.z), Quaternion.identity);
		}
	}
}