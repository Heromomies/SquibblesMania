using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using TMPro;
using UnityEngine;
using Wizama.Hardware.Light;
using Random = UnityEngine.Random;

public class EventManager : MonoBehaviour
{
	[Header("CONDITION RELEASE EVENT")] public List<ConditionReleaseEvent> conditionReleaseEvents;
	
	[Space] [Header("EVENTS")] public List<GameObject> events;

	[HideInInspector] public List<GameObject> cleanList;

	[Space] [Header("MAP ZONE")] public List<GameObject> listZoneNorthWest;
	public List<GameObject> listZoneNorthEst;
	public List<GameObject> listZoneSouthWest;
	public List<GameObject> listZoneSouthEst;

	[Space] [Header("CANVAS")] public GameObject canvasButton;
	public List<TextMeshProUGUI> textToReleaseEvent;

	private ConditionReleaseEvent _condition;
	private float _release;
	private float _numberOfSteps;
	private int _numberOfCondition;
	private PlayerStateManager _player;
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
		DefineCondition();
	}

	void DefineCondition() // Define the condition to activate event
	{
		_numberOfCondition = Random.Range(0, conditionReleaseEvents.Count);
		_condition = conditionReleaseEvents[_numberOfCondition];

		if (_condition.conditions[0].conditionType == Condition.ConditionType.NumberOfSteps)
		{
			textToReleaseEvent[0].text = conditionReleaseEvents[_numberOfCondition].conditions[0].conditionToRelease +
			                             conditionReleaseEvents[_numberOfCondition].conditions[0].numberOfSteps;
		}
		if (_condition.conditions[1].conditionType == Condition.ConditionType.WalkOnCase)
		{
			textToReleaseEvent[1].text = conditionReleaseEvents[_numberOfCondition].conditions[1].conditionToRelease + 
			                             conditionReleaseEvents[_numberOfCondition].conditions[1].numberOfSteps;;

		}
		if(_condition.conditions[2].conditionType == Condition.ConditionType.MoveCase)
		{
			textToReleaseEvent[2].text = conditionReleaseEvents[_numberOfCondition].conditions[2].conditionToRelease +
			                             conditionReleaseEvents[_numberOfCondition].conditions[2].numberOfSteps;; 

		}
		//conditionReleaseEvents.Remove(conditionReleaseEvents[_numberOfCondition]);
	}

	public void AddPointForNumberOfSteps(int i)
	{
		_condition.conditions[0].numberOfSteps -= i;
		textToReleaseEvent[0].text = _condition.conditions[0].conditionToRelease + _condition.conditions[0].numberOfSteps;
		Debug.Log(_condition.conditions[0].conditionToRelease+ (_condition.conditions[0].numberOfSteps -i));
	}
	private void Update()
	{
		NumberOfSteps();
		MoveCase();
		ColorToWalkOn();
	}

	// Update is called once per frame
	void NumberOfSteps() // Function to update conditions and launch the event when the number of steps is reached
	{
		if (_release >= _condition.conditions[_numberOfCondition].numberOfSteps)
		{
			canvasButton.SetActive(true);
			_release = 0;
		}
	}

	void MoveCase() // Function to update conditions and launch the event when the number of case moved on is reached
	{ }

	void ColorToWalkOn() // Function to update conditions and launch the event when the number of case of a certain color is reached
	{ }
	
	public void AddPointToReleaseEvent()
	{
		_release++;
	}

	public void ClickButton(int numberButton)
	{
		switch (numberButton)
		{
			case 0:
				cleanList = listZoneNorthWest;
				Debug.Log(cleanList);
				break;
			case 1:
				cleanList = listZoneNorthEst;
				Debug.Log(cleanList);
				break;
			case 2:
				cleanList = listZoneSouthWest;
				Debug.Log(cleanList);
				break;
			case 3:
				cleanList = listZoneSouthEst;
				Debug.Log(cleanList);
				break;
		}

		canvasButton.SetActive(false);
		events[1].SetActive(true);
		/* int random = Random.Range(0, events.Count);
		 events[random].SetActive(true);*/
	}
}