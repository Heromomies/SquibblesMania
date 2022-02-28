using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventManager : MonoBehaviour
{
	[Space] [Header("EVENTS")] public List<GameObject> events;
	
	[Space] [Header("MAP ZONE")] 
	public List<GameObject> cleanList;
	
	public int dangerousness;

	public LevelOfDanger levelOfDanger;
	
	public enum LevelOfDanger
	{
		LevelOne = 0,
		LevelTwo = 1,
		LevelThree = 2,
		LevelFour = 3
	}
	
	
	#region Singleton

	private static EventManager eventManager;

	public static EventManager Instance => eventManager;
	// Start is called before the first frame update

	private void Awake()
	{
		eventManager = this;
	}

	#endregion
	
	public void CyclePassed() // When a cycle is make, random Number to know if the manager can launch the event
	{
		var randomNumber = Random.Range(0, 100);
		if (randomNumber < 25 * GameManager.Instance.cycleCount)
		{
			LaunchEvent();
			levelOfDanger = LevelOfDanger.LevelOne;
			dangerousness = 0;
			GameManager.Instance.cycleCount = 0;
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