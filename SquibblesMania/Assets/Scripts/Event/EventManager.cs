using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Light;

public class EventManager : MonoBehaviour
{
    [Header("CONDITION RELEASE EVENT")]
    public List<ConditionReleaseEvent> conditionReleaseEvents = new List<ConditionReleaseEvent>();
   // public Component[] components;
    [Space] [Header("EVENTS")]
    public List<GameObject> events;

    [HideInInspector] public List<GameObject> cleanList;
    
    [Space] [Header("MAP ZONE")]
    public List<GameObject> listZoneNorthWest;
    public List<GameObject> listZoneNorthEst;
    public List<GameObject> listZoneSouthWest;
    public List<GameObject> listZoneSouthEst;

    [Space] [Header("CANVAS")]
    public GameObject canvasButton;

    private ConditionReleaseEvent _conditionSo;
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
        _conditionSo = conditionReleaseEvents[random];
        conditionReleaseEvents.Remove(conditionReleaseEvents[random]);
    }

    // Update is called once per frame
    void Update()
    {
        if (_release >= _conditionSo.numberOfSteps)
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
