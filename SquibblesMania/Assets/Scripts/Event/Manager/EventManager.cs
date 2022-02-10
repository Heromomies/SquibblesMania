using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventManager : MonoBehaviour
{
	[Space] [Header("EVENTS")] public List<GameObject> events;

	 public List<GameObject> cleanList;

	[Space] [Header("MAP ZONE")] public List<GameObject> listZoneNorthWest;
	public List<GameObject> listZoneNorthEst;
	public List<GameObject> listZoneSouthWest;
	public List<GameObject> listZoneSouthEst;

	public int dangerousness;
	
	public Slider sliderDangerousness;
	
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

	// Start is called before the first frame update
	void Start()
	{
		cleanList = listZoneNorthWest;
		sliderDangerousness.value = dangerousness;
		DefineCondition();
	}

	public void CyclePassed() // When a cycle is make, random Number to know if the manager can launch the event
	{
		int randomNumber = Random.Range(0, 101);
		if (randomNumber < 34 * GameManager.Instance.cycleCount)
		{
			LaunchEvent();
			levelOfDanger = LevelOfDanger.LevelOne;
			dangerousness = 0;
			sliderDangerousness.value = dangerousness;
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
			sliderDangerousness.value = dangerousness;
		}
	}
	private void Update()
	{
		
		
		// Launch The Event
		
	}
	private void DefineCondition() // Define the condition to activate event
	{
		switch (levelOfDanger)
		{
			case LevelOfDanger.LevelOne : Debug.Log("Level One");
				dangerousness = 0;
				break;
			case LevelOfDanger.LevelTwo : Debug.Log("Level Two");
				dangerousness = 1;
				break;
			case LevelOfDanger.LevelThree : Debug.Log("Level Three");
				dangerousness = 2;
				break;
			case LevelOfDanger.LevelFour : Debug.Log("Level Four");
				dangerousness = 3;
				break;
		}
	}
	public void ClickButton(int numberButton) // Chose the zone where the event will appear 
	{
		switch (numberButton)
		{
			case 0:
				cleanList = listZoneNorthWest;
				break;
			case 1:
				cleanList = listZoneNorthEst;
				break;
			case 2:
				cleanList = listZoneSouthWest;
				break;
			case 3:
				cleanList = listZoneSouthEst;
				break;
		}

		//
	}

	void LaunchEvent() // Launch the event
	{
		foreach (var eventsGo in events)
		{
			eventsGo.SetActive(true);
		}
	}
}