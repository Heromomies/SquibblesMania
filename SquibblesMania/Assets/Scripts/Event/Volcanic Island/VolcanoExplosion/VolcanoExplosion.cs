using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class VolcanoExplosion : MonoBehaviour, IManageEvent
{
	public Color colorOne, colorTwo;
	
	public GameObject particleSystemExplosion;
	public Transform transformToSpawnParticles;

	[HideInInspector] public List<GameObject> cubeOnMap;
	[HideInInspector] public List<GameObject> cubeTouched;
	
	public Rigidbody bulletPrefab;
	public Transform volcanoTransform;

	[Range(0.0f, 3.0f)] public float speed;
	[Range(0.0f, 1.0f)] public float repeatRate;

	private int _turn;

	public Conditions[] conditionsDangerousness;

	[Serializable]
	public struct Conditions
	{
		public int numberOfMeteorite;
	}

	private void OnEnable()
	{
		ShowEvent();
	}

	public void ShowEvent()
	{
		cubeOnMap = EventManager.Instance.cleanList;

		for (int i = 0; i < conditionsDangerousness[EventManager.Instance.dangerousness].numberOfMeteorite; i++)
		{
			int placeOfCube =1;
			EventManager.Instance.cleanList.Remove(EventManager.Instance.cleanList[placeOfCube]);
			
			RandomEvent(placeOfCube);
		}
		
		LaunchEvent();
	}

	public void LaunchEvent()
	{
		Instantiate(particleSystemExplosion, new Vector3(transformToSpawnParticles.position.x,
			transformToSpawnParticles.position.y + 1, transformToSpawnParticles.position.z), Quaternion.identity);
		InvokeRepeating(nameof(LaunchBullet), 0.2f, repeatRate);
	}

	#region Highlight Cubes Who Will Be Touched

	private void RandomEvent(int placeOfCube) // Change the color of the block choose by the random
	{
		cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color = colorOne;

		cubeTouched.Add(cubeOnMap[placeOfCube]);
		_turn = GameManager.Instance.turnCount;
	}

	#endregion

	#region Explosion From The Center

	void LaunchBullet() // Launch the bullets 
	{
		cubeTouched[0].tag = "Black Block";

		var positionVol = volcanoTransform.position;
		Vector3 vo = CalculateVelocity(cubeTouched[0].transform.position, positionVol,
			speed); // Add the velocity to make an effect of parabola for the bullets
		transform.rotation = Quaternion.LookRotation(vo + new Vector3(1, 1, 1));

		cubeTouched.Remove(cubeTouched[0]);

		Rigidbody obj = Instantiate(bulletPrefab, positionVol, Quaternion.identity);
		obj.velocity = vo;
	}

	#endregion

	void Update()
	{
		if (_turn < GameManager.Instance.turnCount)
		{
			_turn = GameManager.Instance.turnCount;
			
			for (int i = 0; i < conditionsDangerousness[EventManager.Instance.dangerousness].numberOfMeteorite; i++)
			{
				cubeTouched[i].GetComponent<Renderer>().material.color = Color.Lerp(colorOne, colorTwo, Mathf.PingPong(Time.time, 1));
			}
		}

		if (cubeTouched.Count <= 0)
		{
			CancelInvoke();
			gameObject.SetActive(false);
		}
	}

	#region CalculateVelocity

	Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float velocity) // Function to make a parabola
	{
		//define the distance x and y first
		Vector3 distance = target - origin;
		Vector3 distanceXZ = distance;
		distanceXZ.Normalize();
		distanceXZ.y = 0;

		//creating a float that represents our distance 
		float sy = distance.y;
		float sxz = distance.magnitude;

		//calculating initial x velocity
		//Vx = x / t

		float vxz = sxz / velocity;

		////calculating initial y velocity
		//Vy0 = y/t + 1/2 * g * t

		float vy = sy / velocity + 0.6f * Mathf.Abs(Physics.gravity.y + 0.7f) * velocity;
		Vector3 result = distanceXZ * vxz;
		result.y = vy;

		return result;
	}

	#endregion
}