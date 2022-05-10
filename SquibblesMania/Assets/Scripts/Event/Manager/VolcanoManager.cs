using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VolcanoManager : MonoBehaviour
{
	[Space] [Header("EVENTS")] public List<GameObject> events;

	public int dangerousness;

	public LevelOfDanger levelOfDanger;

	[Space] [Header("VOLCANO FEEDBACK")] public float duration;
	public float strength;
	
	private Camera _cam;
	
	public enum LevelOfDanger
	{
		LevelOne = 0,
		LevelTwo = 1,
		LevelThree = 2,
		LevelFour = 3
	}

	#region Singleton

	private static VolcanoManager _volcanoManager;

	public static VolcanoManager Instance => _volcanoManager;
	// Start is called before the first frame update

	private void Awake()
	{
		_volcanoManager = this;
		_cam = Camera.main;
	}

	#endregion

	public void CyclePassed() // When a cycle is make, random Number to know if the manager can launch the event
	{
		var randomNumber = Random.Range(0, 100);
		
		if (randomNumber < (25 * dangerousness))
		{
			LaunchEvent();
			levelOfDanger = LevelOfDanger.LevelOne;
			dangerousness = 0;
		}
		else
		{
			switch (levelOfDanger)
			{
				case LevelOfDanger.LevelOne : levelOfDanger = LevelOfDanger.LevelTwo;
					dangerousness = 1;
					break;
				case LevelOfDanger.LevelTwo :levelOfDanger = LevelOfDanger.LevelThree;
					dangerousness = 2;
					break;
				case LevelOfDanger.LevelThree : levelOfDanger = LevelOfDanger.LevelFour;
					dangerousness = 3;
					break;
			}

			foreach (var b in UiManager.Instance.buttonsCameraManager)
			{
				b.gameObject.SetActive(false);
			}
			
			_cam.DOShakePosition(duration, strength, 90, 100);
			_cam.DOShakeRotation(duration, strength, 90, 100);

			StartCoroutine(ActivateButtons());
		}
	}

	IEnumerator ActivateButtons()
	{
		yield return new WaitForSeconds(duration);
		
		foreach (var b in UiManager.Instance.buttonsCameraManager)
		{
			b.gameObject.SetActive(true);
		}
	}
	
	void LaunchEvent() // Launch the event
	{
		foreach (var eventsGo in events)
		{
			eventsGo.SetActive(true);
		}
	}
}