using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovementBlockManager : MonoBehaviour
{
    public GameObject buttonMoveBlockParentObject;
    public bool isMovingBlock;
    private static MovementBlockManager _movementBlockManager;

    public static MovementBlockManager Instance => _movementBlockManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        _movementBlockManager = this;
    }

    public void StartUpPlatform()
    {
        StartCoroutine(PlatformUp());
    }

    public void StartDownPlatform()
    {
        StartCoroutine(PlatformDown());
    }

    IEnumerator PlatformUp()
    {
        
        GroupBlockDetection groupBlockDetection = TouchManager.Instance.blockParent.GetComponent<GroupBlockDetection>();


        Vector3 positionBlockParent = TouchManager.Instance.blockParent.position;

        if (TouchManager.Instance.blockParent.position.y >= 4)
        {
            yield break;
        }


        TouchManager.Instance.blockParent.DOMove(
            new Vector3(positionBlockParent.x, positionBlockParent.y + 1f, positionBlockParent.z),
            0.2f);
        yield return new WaitForSeconds(0.3f);

        //We want to substract action point from the current player if he move up/down the block
        GameManager.Instance.currentPlayerTurn.playerActionPoint--;
        UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
            .playerActionPoint);


        //Move the player with block
        if (groupBlockDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlockDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(
                    new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + 1f, playerOnGroupBlockPos.z), 0.2f);
            }
            yield return new WaitForSeconds(0.3f);
        }

        

        ResetPreviewPlatform();
        isMovingBlock = false;
        TouchManager.Instance.blockParent = null;

        GameManager.Instance.isPathRefresh = true;
        if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);

            foreach (var block in GameManager.Instance.currentPlayerTurn.nextBlockPath)
            {
                block.gameObject.GetComponent<Renderer>().materials[2].color = ResetPreviousBlockColor();
            }
        }
    }

    IEnumerator PlatformDown()
    {
        GroupBlockDetection groupBlockDetection = TouchManager.Instance.blockParent.GetComponent<GroupBlockDetection>();
        if (TouchManager.Instance.blockParent.position.y <= 0)
        {
            yield break;
        }


        Vector3 positionBlockParent = TouchManager.Instance.blockParent.position;
        TouchManager.Instance.blockParent.DOMove(
            new Vector3(positionBlockParent.x, positionBlockParent.y - 1f, positionBlockParent.z),
            0.2f);
        yield return new WaitForSeconds(0.3f);

        GameManager.Instance.currentPlayerTurn.playerActionPoint--;

        UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
            .playerActionPoint);


        //Move the player with block

        if (groupBlockDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlockDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(
                    new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y - 1f, playerOnGroupBlockPos.z), 0.2f);
            }
            yield return new WaitForSeconds(0.3f);
        }

        
        ResetPreviewPlatform();

        isMovingBlock = false;
        TouchManager.Instance.blockParent = null;
        GameManager.Instance.isPathRefresh = true;

        if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);
            foreach (var block in GameManager.Instance.currentPlayerTurn.nextBlockPath)
            {
                block.gameObject.GetComponent<Renderer>().materials[2].color = ResetPreviousBlockColor();
            }
        }
    }


    public Color ResetPreviousBlockColor()
    {
        Material blockCurrentlySelectedMat = TouchManager.Instance.blockCurrentlySelected.GetComponent<Renderer>().materials[2];
        blockCurrentlySelectedMat.color = TouchManager.Instance.blockCurrentlySelectedColor;
        return blockCurrentlySelectedMat.color;
    }

    private void ResetPreviewPlatform()
    {
        if (TouchManager.Instance.blockParent != null)
        {
            buttonMoveBlockParentObject.SetActive(false);
        }
    }
}