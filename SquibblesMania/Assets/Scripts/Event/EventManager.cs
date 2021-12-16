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

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, conditionReleaseEvents.Count);
        _conditionSO = conditionReleaseEvents[random];
        conditionReleaseEvents.Remove(conditionReleaseEvents[random]);
        
        //Debug.Log(_conditionSO);
        
        /*for (int i = 0; i < components.Length; i++)
        {
            components[i] = components[i].GetComponent(typeof(MonoBehaviour));
        }
        foreach(var component in components)
        {
            Debug.Log(component.ToString());
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //TODO search the conditions to activate event with scriptable object
        bool one = false;
        
        if (_conditionSO.numberOfSteps >=7 && !one)
        {
            one = true;
            int random = Random.Range(1, events.Count);
            events[random].SetActive(true);
        }
    }

    public void OnClick()
    {
        _conditionSO.numberOfSteps++;
    }
}
