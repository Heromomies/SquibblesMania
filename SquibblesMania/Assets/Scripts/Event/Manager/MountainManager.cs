using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MountainManager : MonoBehaviour
{
    public List<GameObject> snowGuns;
    
    public int turnBeforeActivateWind; 
    [HideInInspector] public int turn;
    
    #region Singleton

    private static MountainManager _mountainManager;

    public static MountainManager Instance => _mountainManager;
    // Start is called before the first frame update

    private void Awake()
    {
	    _mountainManager = this;
    }

    #endregion
    
    // Start is called before the first frame update
    public void ChangeCycle()
    {
	    foreach (var snowGun in snowGuns)
	    {
		    snowGun.SetActive(false);
	    }
	    
	    var randomNumber = Random.Range(0, snowGuns.Count);
	    snowGuns[randomNumber].SetActive(true);
    }

    public void TurnPassed()
    {
	    
    }
    
    private void Update()
    {
	    if (Input.GetKeyDown(KeyCode.L))
	    {
		    ChangeCycle();
	    }
    }
}
