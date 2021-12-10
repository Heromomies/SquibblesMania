using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStateManager : MonoBehaviour
{
    private EventBaseState _currentState;

    private EruptionState _eruptionState = new EruptionState();
    private SmokeAsheState _smokeAsheState= new SmokeAsheState();
    private RiverState _riverState= new RiverState();

    public GameObject meteorite;

    #region Singleton

    private static EventStateManager _instance;

    public static EventStateManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EventStateManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }    

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        _currentState = _eruptionState;
        
        _currentState.EnterState(this);
    }
}
