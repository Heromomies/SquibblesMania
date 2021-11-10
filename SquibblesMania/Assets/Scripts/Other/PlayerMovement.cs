using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView view;

    
    private static Player _instance;

    public static Player Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Player>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }


    private bool _canTouch = true;
    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            PhotonView photonView = PhotonView.Get(this);
            
            if (Input.GetKeyDown(KeyCode.Q))
            { 
                photonView.RPC("ChangeColor", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                photonView.RPC("ResetColor", RpcTarget.All);
            }
            if (Input.touchCount > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began && _canTouch)
                {
                    photonView.RPC("ChangeColor", RpcTarget.All);
                    _canTouch = false;
                }
                if (Input.touches[0].phase == TouchPhase.Ended && !_canTouch)
                {
                    photonView.RPC("ResetColor", RpcTarget.All);
                    _canTouch = true;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                transform.position += Vector3.up;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.position += Vector3.down;
            }
            
        }
    }

    [PunRPC]
    void ChangeColor()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //GetComponent<Renderer>().material.color = Color.blue;
    }

    [PunRPC]
    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
    
}
