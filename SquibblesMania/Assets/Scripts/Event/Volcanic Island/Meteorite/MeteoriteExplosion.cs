using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class MeteoriteExplosion : MonoBehaviour, IManageEvent
{
	[Header("EVENT")] [SerializeField] private FoldoutValueHolder foldoutValues;

	[Space] [Header("EVENT ANIMATION")] [SerializeField]
	private FoldoutValueHolderEvent foldoutValuesEvent;

	public Color colorOne, colorTwo;
	
	private List<int> _usedValues = new List<int>();

	private int _turn;
	public List<int> _placeOfCube= new List<int>();
	private bool _done;
	
		IEnumerator AnimationVolcano()
	{
		// Create a new Sequence.
		Sequence s = DOTween.Sequence();
		// Change the scale of the object to make it smaller
		s.Append(foldoutValues.volcanoTransform.DOScaleY(-1f, foldoutValuesEvent.durationOfScale).SetRelative().SetEase(Ease.Linear));
		s.Insert(0, foldoutValues.volcanoTransform.DOScaleZ(-0.3f, foldoutValuesEvent.durationOfScale).SetRelative().SetEase(Ease.Linear));
		s.Insert(0, foldoutValues.volcanoTransform.DOScaleX(-0.3f, foldoutValuesEvent.durationOfScale).SetRelative().SetEase(Ease.Linear));

		// Add shake to the object
		s.Insert(foldoutValuesEvent.durationOfScale,
			foldoutValues.volcanoTransform.DOShakePosition(foldoutValuesEvent.durationShake, foldoutValuesEvent.strength, foldoutValuesEvent.vibrato,
				foldoutValuesEvent.randomness));

		// Change the scale of the object to make it bigger
		s.Insert(foldoutValuesEvent.durationOfScale * 3.5f,
			foldoutValues.volcanoTransform.DOScaleY(2f, foldoutValuesEvent.durationOfScale / 2).SetRelative().SetEase(Ease.Linear));
		s.Insert(foldoutValuesEvent.durationOfScale * 3.5f,
			foldoutValues.volcanoTransform.DOScaleZ(1.3f, foldoutValuesEvent.durationOfScale / 2).SetRelative().SetEase(Ease.Linear));
		s.Insert(foldoutValuesEvent.durationOfScale * 3.5f,
			foldoutValues.volcanoTransform.DOScaleX(1.3f, foldoutValuesEvent.durationOfScale / 2).SetRelative().SetEase(Ease.Linear));

		yield return new WaitForSeconds(foldoutValuesEvent.durationOfScale * 3.5f);
		Instantiate(foldoutValuesEvent.particleSystem, new Vector3(foldoutValuesEvent.transformToSpawnParticles.position.x,
			foldoutValuesEvent.transformToSpawnParticles.position.y + 1, foldoutValuesEvent.transformToSpawnParticles.position.z), Quaternion.identity);
		InvokeRepeating(nameof(LaunchBullet), 0.2f, foldoutValues.repeatRate);
	}

	public void ShowEvent()
	{
		foldoutValues.cubeOnMap = EventManager.Instance.cleanList;

		while (foldoutValues.numberOfMeteorite > 0)
		{
			foldoutValues.numberOfMeteorite--;

			int placeOfCube = Random.Range(1, EventManager.Instance.cleanList.Count);
			EventManager.Instance.cleanList.Remove(EventManager.Instance.cleanList[placeOfCube]);

			_placeOfCube.Add(placeOfCube); 
			RandomEvent(placeOfCube);
		}
	}

	public void LaunchEvent()
	{
		_done = true;
		StartCoroutine(AnimationVolcano());
	}

	#region Highlight Cubes Who Will Be Touched

	private void RandomEvent(int placeOfCube) // Change the color of the block choose by the random
	{
		foldoutValues.cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color = colorOne;

		foldoutValues.cubeTouched.Add(foldoutValues.cubeOnMap[placeOfCube]);
		_turn = GameManager.Instance.turnCount;
	}

	#endregion

	#region Explosion From The Center

	void LaunchBullet() // Launch the bullets 
	{
		foldoutValues.cubeTouched[0].tag = "Black Block";

		var positionVol = foldoutValues.volcanoTransform.position;
		Vector3 vo = CalculateVelocity(foldoutValues.cubeTouched[0].transform.position, positionVol,
			foldoutValues.speed); // Add the velocity to make an effect of parabola for the bullets
		transform.rotation = Quaternion.LookRotation(vo + new Vector3(1, 1, 1));

		foldoutValues.cubeTouched.Remove(foldoutValues.cubeTouched[0]);

		Rigidbody obj = Instantiate(foldoutValues.bulletPrefab, positionVol, Quaternion.identity);
		obj.velocity = vo;
	}

	#endregion

	void Update()
	{
		if (_turn < GameManager.Instance.turnCount && !_done)
		{
			_turn = GameManager.Instance.turnCount;
			
			for (int i = 0; i < _placeOfCube.Count; i++)
			{
				foldoutValues.cubeTouched[i].GetComponent<Renderer>().material.color =Color.Lerp(colorOne, colorTwo, Mathf.PingPong(Time.time, 1));
			}
		}

		if (foldoutValues.cubeTouched.Count <= 0)
		{
			CancelInvoke();
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

		float vy = sy / velocity + 0.6f * Mathf.Abs(Physics.gravity.y + 0.5f) * velocity;
		Vector3 result = distanceXZ * vxz;
		result.y = vy;

		return result;
	}

	#endregion

	[Serializable]
	private class FoldoutValueHolderEvent
	{
		public float durationOfScale;

		public float durationShake;
		public Vector3 strength;
		public int vibrato;
		public float randomness;

		public GameObject particleSystem;
		public Transform transformToSpawnParticles;
	}

	[Serializable]
	private class FoldoutValueHolder
	{
		[HideInInspector] public List<GameObject> cubeOnMap;
		[HideInInspector] public List<GameObject> cubeTouched;

		public int numberOfMeteorite;

		public Rigidbody bulletPrefab;
		public Transform volcanoTransform;

		[Range(0.0f, 3.0f)] public float speed;
		[Range(0.0f, 1.0f)] public float repeatRate;
	}
}