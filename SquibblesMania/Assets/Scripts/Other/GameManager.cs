using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public List<string> userId = new List<string>();

    private void Start()
    {
        Debug.Log("UserId=" + PhotonNetwork.AuthValues.UserId);
        userId.Add(PhotonNetwork.AuthValues.UserId);
    }
}