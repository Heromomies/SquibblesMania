using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class Player : MonoBehaviour
{
  //  private PlayerInputsActions _playerInputsActions;


//    [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCameraPlayers;

    public bool isChangedCam;

    public int indexChangeCam;
   

    // Start is called before the first frame update
    void Awake()
    {
    
     
    }

   

 /*   private void RotateCaméraOnstarted(InputAction.CallbackContext context)
    {
        isChangedCam = context.ReadValueAsButton();
    }*/

    // Update is called once per frame
    void Update()
    {
        //ChangeViewCam();
    }

/*    public void ChangeViewCam()
    {
        if (isChangedCam)
        {
            isChangedCam = false;

            foreach (CinemachineVirtualCamera camera in cinemachineVirtualCameraPlayers)
            {
                camera.Priority = 0;
            }

            int changeCamPlayerIndex = indexChangeCam;
            cinemachineVirtualCameraPlayers[changeCamPlayerIndex].Priority = 1;

            indexChangeCam++;

            if (indexChangeCam > cinemachineVirtualCameraPlayers.Length - 1)
            {
                indexChangeCam = 0;
            }
        }
    }

    private void OnEnable()
    {
        _playerInputsActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputsActions.Disable();
    }*/
}