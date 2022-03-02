using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerActionPointCardState : PlayerBaseState
{
    public List<Transform> previewPath = new List<Transform>();
    private int _actionPointText;
    public Color blocBaseEmissiveColor;
    private bool isPreviewPathDone;
    private WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.5f);
    //The state when player use is card action point
    public override void EnterState(PlayerStateManager player)
    {
        player.nextBlockPath.Clear();
        blocBaseEmissiveColor = player.currentBlockPlayerOn.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
        player.currentBlockPlayerOn.GetComponent<Node>().isActive = true;
        previewPath.Clear();
        PreviewPath(player.playerActionPoint, player);
    }

    #region PREVIEW

    public void PreviewPath(int actionPoint, PlayerStateManager player)
    {
        List<Transform> possiblePath = new List<Transform>();
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> finalPreviewPath = new List<Transform>();

        int indexBlockNearby = 0;

        //Take the base color of the block
        if (player.currentBlockPlayerOn != null)
        {
            player.currentBlockPlayerOn.gameObject.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", blocBaseEmissiveColor);
            TouchManager.Instance.blockCurrentlyBaseColor = blocBaseEmissiveColor;
        }

        //Foreach possible path compared to the block wich player is currently on
        foreach (GamePath path in player.currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            Node actualNode = player.currentBlockPlayerOn.GetComponent<Node>();
            bool isNextPathActive = path.nextPath.GetComponent<Node>().isActive;

            if (path.isActive && isNextPathActive && actualNode.isActive)
            {
                possiblePath.Add(path.nextPath);
                finalPreviewPath.Add(path.nextPath);
                path.nextPath.GetComponent<Node>().previousBlock = player.currentBlockPlayerOn;
            }
        }

        finalPreviewPath.Add(player.currentBlockPlayerOn);
    
            
        //We add in our list of past blocks, the block which the player is currently on
        pastBlocks.Add(player.currentBlockPlayerOn);

        ExplorePreviewPath(possiblePath, pastBlocks, finalPreviewPath, indexBlockNearby, actionPoint, player);
        
    }

    private void ExplorePreviewPath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, int indexBlockNearby, int actionPoint, PlayerStateManager playerStateManager)
    {
        playerStateManager.nextBlockPath = finalPreviewPath;

        indexBlockNearby++;
     
        //If our current block is == to the player selected block then out of the loop
        if (indexBlockNearby == actionPoint)
        {
            ResetColorPreviewPath(finalPreviewPath, blocBaseEmissiveColor);
            return;
        }

        //The blocks we want to check
        List<Transform> currentCheckedBlocks = new List<Transform>();

        // if we have multiple block around the player
        if (nextBlocksPath.Count > 1)
        {
            for (int i = 0; i < nextBlocksPath.Count; i++)
            {
                currentCheckedBlocks.Add(nextBlocksPath[i]);
            }

            nextBlocksPath.Remove(currentCheckedBlocks[0]);
        }
        else
        {
            currentCheckedBlocks.Add(nextBlocksPath[0]);
            nextBlocksPath.Remove(currentCheckedBlocks[0]);
        }
        

        CheckPossiblePaths(currentCheckedBlocks, previousBlocksPath, finalPreviewPath, nextBlocksPath, playerStateManager);

        for (int i = 0; i < currentCheckedBlocks.Count; i++)
        {
            //We add in our list of path who are already visited, our currently checked blocks
            previousBlocksPath.Add(currentCheckedBlocks[i]);
        }

        //If in our list, he stay a element, we restart the void
        if (nextBlocksPath.Any())
        {
            ExplorePreviewPath(nextBlocksPath, previousBlocksPath, finalPreviewPath, indexBlockNearby, actionPoint,
                playerStateManager);
        }
    }

    #endregion

  public void ResetColorPreviewPath(List<Transform> finalPreviewPath, Color color)
    {
        foreach (var bloc in finalPreviewPath)
        {
            Material blocSquareMat = bloc.GetComponent<Renderer>().materials[2];
            blocSquareMat.SetColor("_EmissionColor", color);
        }
        previewPath = finalPreviewPath;
    }
    

    void CheckPossiblePaths(List<Transform> currentCheckedBlocks, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, List<Transform> nextBlocksPath, PlayerStateManager player)
    {
        Debug.Log("hell world");
        //Foreach currents checked block in our list
        foreach (Transform checkedBlock in currentCheckedBlocks)
        {
            //Foreach possible path in our currentCheckedBlock
            foreach (GamePath path in checkedBlock.GetComponent<Node>().possiblePath)
            {
                Vector3 pathParentPos = path.nextPath.transform.parent.position;
                Vector3 checkedBlockPos = checkedBlock.transform.position;
                bool isNextPathActive = path.nextPath.GetComponent<Node>().isActive;

                //We look if in our list of previousBlockPath, she's not already contains the next block and if the next block is active
                if (!previousBlocksPath.Contains(path.nextPath) && path.isActive && isNextPathActive &&
                    PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position) &&
                    CurrentCheckedBlocPosComparedToNextBlocPos(checkedBlockPos, path.nextPath.transform.position))
                {
                    //We add in our list the next block
                    nextBlocksPath.Add(path.nextPath);
                    finalPreviewPath.Add(path.nextPath);
                    //We assign the previous block to our currently block
                    path.nextPath.GetComponent<Node>().previousBlock = checkedBlock;
                }
            }
        }
       
    }

    bool CurrentCheckedBlocPosComparedToNextBlocPos(Vector3 checkedBlocPos, Vector3 nextBlocPoS)
    {
        return checkedBlocPos.y - nextBlocPoS.y < 0.1f && checkedBlocPos.y - nextBlocPoS.y > -0.1f;
    }

    bool PathParentPosComparedToPlayerPos(Vector3 pathParentPos, Vector3 playerPos)
    {
        return pathParentPos.y + 2.5f - playerPos.y > -0.1f && pathParentPos.y + 2.5f - playerPos.y < 0.1f;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        //Update the preview Path of the player 
        if (player.playerActionPoint > 0 && player.isPlayerInActionCardState)
        {
            PulsingBloc.PulsingEmissiveColorSquareBlocList(blocBaseEmissiveColor, Color.black, player.nextBlockPath, 0.4f);
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = false;
        ResetColorPreviewPath(player.finalPathFinding, blocBaseEmissiveColor);
        player.indicatorPlayer.SetActive(false);
        //Switch to next player of another team to play
        switch (player.playerNumber)
        {
            case 0: GameManager.Instance.ChangePlayerTurn(1); break;
            case 1: GameManager.Instance.ChangePlayerTurn(2); break;
            case 2: GameManager.Instance.ChangePlayerTurn(3); break;
            case 3: GameManager.Instance.ChangePlayerTurn(0); break;
        }
    }

    public void FindPath(PlayerStateManager player)
    {
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> nextBlocks = new List<Transform>();

        //Foreach possible path compared to the block wich player is currently on

        foreach (GamePath path in player.currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            Vector3 pathParentPos = path.nextPath.transform.parent.position;
            bool isNextPathActive = path.nextPath.GetComponent<Node>().isActive;
            //If we have a path who is activated then we add it to the list of nextBlockPath
            if (path.isActive && isNextPathActive && PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position))
            {
                nextBlocks.Add(path.nextPath);

                //In our path element we assign our previous block to the block wich player is currently on
                path.nextPath.GetComponent<Node>().previousBlock = player.currentBlockPlayerOn;
            }
        }


        //We add in our list of past blocks, the block which the player is currently on
        pastBlocks.Add(player.currentBlockPlayerOn);
        //We explore our pathfinding

        ExplorePath(nextBlocks, pastBlocks, player);
        BuildPath(player);
    }

    private void ExplorePath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath,
        PlayerStateManager player)
    {
        //The block wich the player is currently on
        Transform currentBlock = nextBlocksPath[0];


        nextBlocksPath.Remove(currentBlock);
        //If our current block is = to the player selected block then out of the loop
        if (currentBlock == player.currentTouchBlock)
        {
            //Player arrive to the destination
            return;
        }

        //Foreach possible path in our currentBlock
        foreach (GamePath path in currentBlock.GetComponent<Node>().possiblePath)
        {
            Vector3 pathParentPos = path.nextPath.transform.parent.position;
            bool isCurrentBlockActive = currentBlock.GetComponent<Node>().isActive;
            //We look if in our list of previousBlockPath, she's not already contains the next block and if the next block is active
            if (!previousBlocksPath.Contains(path.nextPath) && path.isActive && isCurrentBlockActive &&
                PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position))
            {
                //We add in our list the next block
                nextBlocksPath.Add(path.nextPath);

                //We assign the previous block to our currently block
                path.nextPath.GetComponent<Node>().previousBlock = currentBlock;
            }
        }

        //We add in our list of path who are already visited, our currently block
        previousBlocksPath.Add(currentBlock);

        //If in our list, he stay a element, we restart the void
        if (nextBlocksPath.Any())
        {
            ExplorePath(nextBlocksPath, previousBlocksPath, player);
        }
    }

    private void BuildPath(PlayerStateManager player)
    {
        //The block currently selectionned by the player
        Transform block = player.currentTouchBlock;

        //We remove the player from the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = player.currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Remove(player.gameObject.transform);


        //While the player selected block is != to the block wich player supposed to be 
        while (block != player.currentBlockPlayerOn)
        {
            //We add this block to our list final pathfinding
            player.finalPathFinding.Add(block);

            //If in our selected block, the precedent block is not nul then the block become the past block
            if (block.GetComponent<Node>().previousBlock != null)
            {
                block = block.GetComponent<Node>().previousBlock;
            }
            else
            {
                return;
            }
        }

        player.finalPathFinding.Insert(0, player.currentTouchBlock);


        if (!player.walking)
        {
            player.walking = true;
            player.StartCoroutine(FollowPath(player));
        }
    }

    //Movement of player
    private IEnumerator FollowPath(PlayerStateManager player)
    {
        int movementPlayer = 0;

        _actionPointText = player.playerActionPoint;
        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {
            Vector3 walkPoint = player.finalPathFinding[i].GetComponent<Node>().GetWalkPoint();
            if (movementPlayer < player.playerActionPoint)
            {
                Vector3 movePos = walkPoint + new Vector3(0, 1, 0);
                Vector3 direction = (movePos - player.transform.position).normalized;

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                player.transform.DOMove(movePos, player.timeMoveSpeed);
                player.transform.DORotateQuaternion(Quaternion.Euler(0,targetAngle,0), player.timeRotateSpeed);
                player.finalPathFinding.Remove(player.finalPathFinding[i]);
                _actionPointText--;
                UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(_actionPointText);
                movementPlayer++;
                yield return _timeBetweenPlayerMovement;
            }
        }

        player.playerActionPoint = _actionPointText;
        Clear(player);
    }

    private void Clear(PlayerStateManager player)
    {
        Node currentNodePlayerOn = player.currentBlockPlayerOn.GetComponent<Node>();
        //We add the player to the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = currentNodePlayerOn.groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Add(player.gameObject.transform);

        //Foreach block in our finalpathfinding we reset the previous blocks at the end of the loop
        foreach (Transform t in player.finalPathFinding)
        {
            t.GetComponent<Node>().previousBlock = null;
        }

        ResetColorPreviewPath(player.nextBlockPath, blocBaseEmissiveColor);

        player.finalPathFinding.Clear();
        player.walking = false;
        
        
        if (EndZoneManager.Instance != null)
        {
            EndZoneManager.Instance.CheckPlayersTeam();
            EndZoneManager.Instance.PlayersIsOnEndZone();
        }
        
        if (player.playerActionPoint > 0)
        {
            EnterState(player);
           
        }
        else
        {
            currentNodePlayerOn.isActive = false;
            if (NFCManager.Instance.hasRemovedCard)
            {
                NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(false);
                NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(false);
                UiManager.Instance.buttonNextTurn.SetActive(true);
            }
            else
            {
                switch (GameManager.Instance.actualCamPreset.presetNumber)
                {
                    case 1: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(true); break;
                    case 2: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(true); break;
                    case 3: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(true); break;
                    case 4: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(true); break;
                }
            }
        }
    }
}