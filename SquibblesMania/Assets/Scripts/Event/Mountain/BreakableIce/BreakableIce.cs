using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakableIce : MonoBehaviour
{
    private Node _node;
    public GameObject shaderReplaceObject;
    
    public int minTurnBeforeRespawn, maxTurnBeforeRespawn;
    
    private GameObject _nodeGo;
    private MeshRenderer _nodeMeshRenderer;
    private int _turn;
    private int _randomNumber;
    [HideInInspector] public bool checkCondition;

#if UNITY_EDITOR

    private void OnValidate()
    {
        _node = GetComponentInParent<Node>();
        _nodeGo = _node.gameObject;
        _nodeMeshRenderer = _nodeGo.GetComponent<MeshRenderer>();
    }

#endif
    
    private void Start()
    {
        shaderReplaceObject = Instantiate(shaderReplaceObject, _nodeGo.transform.position, Quaternion.identity, _nodeGo.transform);
        shaderReplaceObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BlocModification();
        }
    }
    
    private void RespawnIce()
    {
        _randomNumber = Random.Range(minTurnBeforeRespawn, maxTurnBeforeRespawn);
        _turn = GameManager.Instance.turnCount;
    }

    public void CheckTurnBeforeRespawn()
    {
        if ( GameManager.Instance.turnCount >= _randomNumber + _turn)
        {
            BlocModificationReverse();
        }
    }
    
    private void BlocModification()
    {
        shaderReplaceObject.SetActive(true) ;
        checkCondition = true;
        _node.isActive = false;
        _nodeMeshRenderer.enabled = false;
        RespawnIce();
    }
    
    private void BlocModificationReverse()
    {
        shaderReplaceObject.SetActive(false);
        checkCondition = false;
        _node.isActive = true;
        _nodeMeshRenderer.enabled = true;
    }
}
