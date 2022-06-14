using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MountainManager : MonoBehaviour
{
	[Header("SNOW GUNS")][Space (10)]
    public List<GameObject> snowGuns;
   
    [Header("WIND")][Space (10)]
    public PolarWind wind;
    public int maxTurnBeforeWindApparition;

    [Header("SNOW")][Space (10)]
    public GameObject snow;
    public int maxTurnBeforeSnowAppear;
    
    [Header("POWDERS BLOCS")] [Space (10)]
    public List<GroupBlockDetection> powdersBlocs;
    
    [Header("BREAKABLE ICE")][Space (10)]
    public List<BreakableIce> breakableIce;

    #region Singleton

    private static MountainManager _mountainManager;

    public static MountainManager Instance => _mountainManager;
    // Start is called before the first frame update

    private void Awake()
    {
	    _mountainManager = this;
    }

    #endregion

    private IEnumerator Start()
    {
	    yield return new WaitForSeconds(0.2f);
	    
	    foreach (var g in  GameManager.Instance.allBlocParents)
	    {
		    var blockParentEventType = g.GetComponent<BlockParentEventType>().typeOfBloc;

		    if (blockParentEventType == BlockParentEventType.TypeOfBloc.Powder)
		    {
			     powdersBlocs.Add(g.GetComponent<GroupBlockDetection>()); 
		    }
		    else if (blockParentEventType == BlockParentEventType.TypeOfBloc.BreakableIce)
		    {
			    var j = g.GetComponentsInChildren<Node>();
			    
			    for (int i = 0; i < j.Length; i++)
			    {
				    GameObject bIce = PoolManager.Instance.SpawnObjectFromPool("BreakableIce", j[i].transform.position + new Vector3(0,1,0), Quaternion.identity, j[i].transform);
				    breakableIce.Add(bIce.GetComponent<BreakableIce>());
			    }
		    }
	    }
    }
    
    public void ChangeCycle() // Change a cycle
    {
	    var boo = AreSnowGunsInactive();

	    if (boo)
	    {
		    foreach (var snowGun in snowGuns)
		    {
			    snowGun.SetActive(false);
		    }
	    
		    var randomNumber = Random.Range(0, snowGuns.Count);
		    snowGuns[randomNumber].SetActive(true);
		    
		    AudioManager.Instance.Play("CanonOpen");
	    }
    }

    bool AreSnowGunsInactive() // Check if one of the snow guns are active, if yes, don't spawn one
    {
	    foreach(var snow in snowGuns) {
		    if(snow.activeSelf) {
			    return false;
		    }
	    }
	    return true;
    }
    
    public void ChangeTurn() // Every time we change a turn
    {
	    foreach (var p in powdersBlocs)
	    {
		    if (p.playersOnGroupBlock.Count > 0 && p.gameObject.transform.position.y > GameManager.Instance.minHeightBlocMovement)
		    {
			    p.gameObject.transform.position += Vector3.down;
			    AudioManager.Instance.Play("Snow");
		    }
	    }

	    foreach (var b in breakableIce)
	    {
		    if (b.checkCondition)
		    {
			    b.CheckTurnBeforeRespawn();
		    }
	    }
	    
	    RandomActivateWind();
	    RandomActivateSnow();
    }

    void RandomActivateWind() // Random function to activate wind
    {
	    if (!wind.gameObject.activeSelf)
	    {
		    var randomNumber = Random.Range(0, maxTurnBeforeWindApparition);
		    if (randomNumber == (int)maxTurnBeforeWindApparition / 2)
		    {
			    wind.gameObject.SetActive(true);
		    }
	    }
	    else
	    {
		    wind.LaunchEvent();
	    }
    }
    
    void RandomActivateSnow() // Random function to activate snow
    {
	    if (!snow.activeSelf)
	    {
		    var randomNumber = Random.Range(0, maxTurnBeforeSnowAppear);
		    if (randomNumber == (int)maxTurnBeforeSnowAppear / 2)
		    {
			    snow.gameObject.SetActive(true);
		    }
	    }   
    }
    
    
#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			wind.gameObject.SetActive(true);
		}
	}
#endif
    
   
}
