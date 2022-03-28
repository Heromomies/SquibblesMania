using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class StunPlayerEasily : MonoBehaviour
{
	public void Update()
    {
	    if (Input.GetKeyDown(KeyCode.T))
	    {
		    var secondPlayer = GameManager.Instance.players[1];
		    var posSp = secondPlayer.transform.position;
		    PoolManager.Instance.SpawnObjectFromPool("Meteorite", new Vector3(posSp.x, posSp.y + 3f, posSp.z), Quaternion.identity, null);
	    }
    }
}
