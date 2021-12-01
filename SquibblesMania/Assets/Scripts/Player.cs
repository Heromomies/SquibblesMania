using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointPlayerText;
    [SerializeField] private int randomCardNumber;
    void Awake()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (randomCardNumber > 0)
        {
            Transform playerNextPos =  MapGenerator.Instance.GetPlatformFromPosition(new Vector3(randomCardNumber, 1, randomCardNumber));
            transform.DOMove(playerNextPos.position, 0.5f);
        }
      

    }

    public void RandomCardUse()
    {
        //On choisit un nombre aléatoire pour les points d'action
        randomCardNumber = Random.Range(1, 5+1);
        actionPointPlayerText.text = randomCardNumber.ToString("0");
        
    }
}