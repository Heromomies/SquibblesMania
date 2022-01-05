using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Light;

public class EventManager : MonoBehaviour
{
    public List<ConditionReleaseEvent> conditionReleaseEvents = new List<ConditionReleaseEvent>();
   // public Component[] components;

    public List<GameObject> events;
   
    private ConditionReleaseEvent _conditionSO;

    private float _release;
    
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
        //TODO search the conditions to activate event with scriptable object

        if (_release >= _conditionSO.numberOfSteps)
        {
            int random = Random.Range(1, events.Count);
            events[random].SetActive(true);

            _release = 0;
        }
    }

    public void OnClick()
    {
        _release++;
    }
}
