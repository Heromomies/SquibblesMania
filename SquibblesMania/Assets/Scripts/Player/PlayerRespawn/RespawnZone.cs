using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class RespawnZone : MonoBehaviour
{
    #region UnityEditor
#if UNITY_EDITOR
    private void OnValidate()
    {
        SetUp();
    }
    private void Reset()
    {
        SetUp();
    }

    void SetUp()
    {
        var transformPosition = transform.position;
        transformPosition.y = GameObject.Find("GameManager").GetComponent<GameManager>().minHeightBlocMovement;
        gameObject.transform.position = transformPosition;
    }
#endif

    

    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        
        
        if (other.TryGetComponent(out PlayerStateManager player))
        {
            RespawnManager.Instance.RespawnPlayer(player);
        }

    }
}
