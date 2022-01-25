using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpulsionPower : MonoBehaviour, IManagePower
{
    public void LaunchPower()
    {
        switch (PowerManager.Instance.raycastPos)
        {
            case 0:
                PowerManager.Instance.hitRaycast.transform.position += Vector3.back;
                break;
            case 1:
                PowerManager.Instance.hitRaycast.transform.position += Vector3.forward;
                break;
            case 2:
                PowerManager.Instance.hitRaycast.transform.position -= Vector3.left;
                break;
            case 3:
                PowerManager.Instance.hitRaycast.transform.position -= Vector3.right;
                break;
        }
        //GameManager.Instance.currentPlayerTurn.PlayerPowerCardState.ExitState(GameManager.Instance.currentPlayerTurn);
    }
}