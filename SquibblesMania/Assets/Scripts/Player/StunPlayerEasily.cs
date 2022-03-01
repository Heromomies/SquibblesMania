using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPlayerEasily : MonoBehaviour
{
    public GameObject stunPlayerMeteorite;

    public void ClickButton()
    {
	    var secondPlayer = GameManager.Instance.players[1];
	    var posSp = secondPlayer.transform.position;
	    Instantiate(stunPlayerMeteorite, new Vector3(posSp.x, posSp.y + 3f, posSp.z), Quaternion.identity);
        
    }
}
