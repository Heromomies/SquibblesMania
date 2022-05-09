using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VolcanoExplosion : MonoBehaviour, IManageEvent
{
	[Header("PARTICLE SYSTEM")] public GameObject particleSystemExplosion;
	[Space] public List<GameObject> cubeOnMap;
	public List<GameObject> cubeTouched;
	public GameObject smokeEffect;
	
	[Space]
	[Header("BULLET AND SPAWN")] 
	public GameObject volcanoGameObject;
	public Transform volcanoTransform;
	public Transform bulletParent;

	[Space] [Header("BULLET SETTINGS")] [Range(0.0f, 3.0f)]
	public float speed;

	[Range(0.0f, 1.0f)] public float repeatRate;

	private Animator _animVolcano;
	
	[Header("CONDITIONS DANGEROUSNESS")] public Conditions[] conditionsDangerousness;

	[Serializable]
	public struct Conditions
	{
		public int numberOfMeteorite;
	}

	private void OnEnable()
	{
		_animVolcano = volcanoGameObject.GetComponent<Animator>();
		ShowEvent();
	}

	public void ShowEvent() // Chose the bloc 
	{
		cubeOnMap = GameManager.Instance.cleanList;

		smokeEffect.SetActive(false);
		
		for (int i = 0; i < conditionsDangerousness[VolcanoManager.Instance.dangerousness].numberOfMeteorite; i++)
		{
			int placeOfCube = Random.Range(0, cubeOnMap.Count - conditionsDangerousness[VolcanoManager.Instance.dangerousness].numberOfMeteorite);
			GameManager.Instance.cleanList.Remove(cubeOnMap[placeOfCube]);

			RandomEvent(placeOfCube);
		}

		LaunchEvent();
	}

	public void LaunchEvent() // Launch the bullet's function
	{
		GameObject ps = Instantiate(particleSystemExplosion, new Vector3(volcanoTransform.position.x,
			volcanoTransform.position.y + 1, volcanoTransform.position.z), Quaternion.identity);

		Destroy(ps, 5f);

		_animVolcano.enabled = true;
		InvokeRepeating(nameof(LaunchBullet), 0.5f, repeatRate);
	}

	#region Highlight Cubes Who Will Be Touched

	private void RandomEvent(int placeOfCube) // Change the color of the block choose by the random
	{
		//Remove end zone block frome the cubeOnMap List
		if (EndZoneManager.Instance != null)
		{
			List<Transform> endZoneBlockChild = EndZoneManager.Instance.blocksChild;

			foreach (var blockChild in endZoneBlockChild)
			{
				if (cubeOnMap.Contains(blockChild.gameObject))
				{
					cubeOnMap.Remove(blockChild.gameObject);
				}
			}
		}


		cubeTouched.Add(cubeOnMap[placeOfCube]);
	}

	#endregion

	#region Explosion From The Center

	void LaunchBullet() // Launch the bullets 
	{
		cubeTouched[0].layer = 7;
		
		var positionVol = volcanoTransform.position;
		Vector3 vo = CalculateVelocity(cubeTouched[0].transform.position - transform.position, positionVol,
			speed); // Add the velocity to make an effect of parabola for the bullets
		transform.rotation = Quaternion.LookRotation(vo + new Vector3(1, 1, 1));

		GameObject obj = PoolManager.Instance.SpawnObjectFromPool("Meteorite", positionVol, Quaternion.identity, bulletParent);
		PoolManager.Instance.SpawnObjectFromPool("ParticleLavaProjection", positionVol, Quaternion.identity, bulletParent);
		obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		obj.GetComponent<Rigidbody>().velocity = vo;

		obj.GetComponent<Meteorite>().stopRotating = false;

		AudioManager.Instance.Play("FireballStart");

		cubeTouched.Remove(cubeTouched[0]);
		
		if (cubeTouched.Count <= 0)
		{
			_animVolcano.enabled = false;
			CancelInvoke();
			smokeEffect.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	#endregion
	
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