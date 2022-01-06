using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Light;

public class EventManager : MonoBehaviour
{
    public List<ConditionReleaseEvent> conditionReleaseEvents = new List<ConditionReleaseEvent>();
   // public Component[] components;

    public List<GameObject> events;

     public List<GameObject> cleanList;
    
    public List<GameObject> lOne;
    public List<GameObject> lTwo;
    public List<GameObject> lThree;
    public List<GameObject> lFour;

    public GameObject canvasButton;

    private ConditionReleaseEvent _conditionSO;

    private float _release;

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
        int random = Random.Range(1, conditionReleaseEvents.Count);
        _conditionSO = conditionReleaseEvents[random];
        conditionReleaseEvents.Remove(conditionReleaseEvents[random]);
    }

    // Update is called once per frame
    void Update()
    {
        if (_release >= _conditionSO.numberOfSteps)
        {
            canvasButton.SetActive(true);
            _release = 0;
        }
    }

    public void AddPointToReleaseEvent()
    {
        _release++;
    }

    public void ClickButton(int numberButton)
    {
        switch (numberButton)
        {
            case 0:
                cleanList = lOne;
                Debug.Log(cleanList);
                break;
            case 1:
                cleanList = lTwo;
                Debug.Log(cleanList);
                break;
            case 2:
                cleanList = lThree;
                Debug.Log(cleanList);
                break;
            case 3:
                cleanList = lFour;
                Debug.Log(cleanList);
                break;
        }
        canvasButton.SetActive(false);
        events[0].SetActive(true);
       /* int random = Random.Range(0, events.Count);
        events[random].SetActive(true);*/
    }
}
