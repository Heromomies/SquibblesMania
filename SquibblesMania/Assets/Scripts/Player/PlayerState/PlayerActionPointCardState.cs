using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerActionPointCardState : PlayerBaseState
{
   
    private int _actionPointText;
    private List<GameObject> pathObjects = new List<GameObject>();
    private List<Transform> previewPath = new List<Transform>();
   

    private WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.5f);
    
    

    //The state when player use is card action point
    public override void EnterState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = true;
        player.nextBlockPath.Clear();  
        ResetPreviewPath(player);
        player.currentBlockPlayerOn.GetComponent<Node>().isActive = true;
        PreviewPath(player.playerActionPoint, player);
    }

    #region PREVIEW

    private void PreviewPath(int actionPoint, PlayerStateManager player)
    {
        List<Transform> possiblePath = new List<Transform>();
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> finalPreviewPath = new List<Transform>();

        int indexBlockNearby = 0;

        if (actionPoint > 0)
        {
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
    }

    private void ExplorePreviewPath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, int indexBlockNearby,
        int actionPoint, PlayerStateManager playerStateManager)
    {
        playerStateManager.nextBlockPath = finalPreviewPath;
        indexBlockNearby++;
        //If our current block is == to the player selected block then out of the loop
        if (indexBlockNearby == actionPoint)
        {
            PreviewPathSpawnGameObjects(finalPreviewPath);
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


        for (int i = 0; i < currentCheckedBlocks.Count; i++)
        {
            //We add in our list of path who are already visited, our currently checked blocks
            previousBlocksPath.Add(currentCheckedBlocks[i]);
        }

        CheckPossiblePaths(currentCheckedBlocks, previousBlocksPath, finalPreviewPath, nextBlocksPath,
            playerStateManager);


        if (nextBlocksPath.Any())
        {
            ExplorePreviewPath(nextBlocksPath, previousBlocksPath, finalPreviewPath, indexBlockNearby, actionPoint,
                playerStateManager);
        }
        else
        {
            PreviewPathSpawnGameObjects(finalPreviewPath);
        }
    }

    #endregion

    private void PreviewPathSpawnGameObjects(List<Transform> finalPreviewPath)
    {
        for (int i = 0; i < finalPreviewPath.Count; i++)
        {
            var bPos = finalPreviewPath[i].position;
            var goPathObject = PoolManager.Instance.SpawnObjectFromPool("PlaneShowPath",
                new Vector3(bPos.x, bPos.y + 1.01f, bPos.z), Quaternion.identity, null);
            pathObjects.Add(goPathObject);
        }
        
    }

    void CheckPossiblePaths(List<Transform> currentCheckedBlocks, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, List<Transform> nextBlocksPath, PlayerStateManager player)
    {
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
                    CurrentCheckedBlocPosComparedToNextBlocPos(checkedBlockPos, path.nextPath.transform.position) &&
                    !finalPreviewPath.Contains(path.nextPath))
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

    public bool PathParentPosComparedToPlayerPos(Vector3 pathParentPos, Vector3 playerPos)
    {
        return pathParentPos.y + 2.5f - playerPos.y > -0.1f && pathParentPos.y + 2.5f - playerPos.y < 0.1f;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = false;
        player.indicatorPlayer.SetActive(false);

        foreach (var obj in pathObjects)
        {
            obj.SetActive(false);
        }

        pathObjects.Clear();

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
            if (path.isActive && isNextPathActive &&
                PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position))
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
        

        if (PlayerMovementManager.Instance.isPlayerPreviewPath)
        {
            player.StartCoroutine(ShowPreviewPath(player));
        }
        
        player.finalPathFinding.Insert(0, player.currentTouchBlock);
    }

    private IEnumerator ShowPreviewPath(PlayerStateManager player)
    {
        var instanceGhostPlayer = PlayerMovementManager.Instance.ghostPlayer;
        for (int i = player.finalPathFinding.Count -1; i > 0; i--)
        {
            var spawnBulletPoint = player.finalPathFinding[i].GetComponent<Node>().GetWalkPoint();
            var sphere = PoolManager.Instance.SpawnObjectFromPool("SphereShowPath", new Vector3(spawnBulletPoint.x, spawnBulletPoint.y + 1.2f, spawnBulletPoint.z), Quaternion.identity, null);
            previewPath.Add(sphere.transform);
        }
        var ghostPlayerPos = player.currentTouchBlock.GetComponent<Node>().GetWalkPoint() + new Vector3(0,0.5f,0);
        instanceGhostPlayer.SetActive(true);
        instanceGhostPlayer.transform.position = ghostPlayerPos;
        previewPath.Add(instanceGhostPlayer.transform);
        yield return null;
    }

    public IEnumerator BeginFollowPath(PlayerStateManager player)
    {
        //Last index of the list finalPathfinding
        int index = player.finalPathFinding.Count - 1;
        
        Vector3 lastDirection = (player.finalPathFinding[index].transform.position - player.currentBlockPlayerOn.position).normalized;
        
        
        UiManager.Instance.SpawnTextActionPointPopUp(player.transform);
        _actionPointText = player.playerActionPoint;
        
        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {
            
            if (i < player.finalPathFinding.Count-1)
            {
                var firstBloc = player.finalPathFinding[index];
                var secondBloc = player.finalPathFinding[index-1];
                
                var blocDirection = (secondBloc.position - firstBloc.position).normalized;

                //Si la dernière direction n'est pas égale a la direction actuel du bloc 1
                if (lastDirection != blocDirection)
                {
                    lastDirection = blocDirection;
                } 
                else
                {
                    player.finalPathFinding.Remove(firstBloc);
                    _actionPointText--;
                    UpdateActionPointTextPopUp(_actionPointText);
                }
                index--;
            }
        }
        yield return null;
        player.playerActionPoint = _actionPointText;
        player.StartCoroutine(FollowPath(player));
    }
    //Movement of player
    private IEnumerator FollowPath(PlayerStateManager player)
    {
        //We remove the player from the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = player.currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Remove(player.gameObject.transform);

        NFCManager.Instance.displacementActivated = true;
        var movementPlayer = 0;
        _actionPointText = player.playerActionPoint;

        UiManager.Instance.SpawnTextActionPointPopUp(player.transform);

        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {
            var walkPoint = player.finalPathFinding[i].GetComponent<Node>().GetWalkPoint();
            if (movementPlayer < player.playerActionPoint)
            {
                var movePos = walkPoint + new Vector3(0, 1, 0);
                var direction = (movePos - player.transform.position).normalized;
                var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
               
                player.transform.DOMove(movePos, player.timeMoveSpeed);
                
                player.transform.DORotateQuaternion(Quaternion.Euler(0, targetAngle, 0), player.timeRotateSpeed);
                //player.finalPathFinding.Remove(player.finalPathFinding[i]);
                _actionPointText--;
                UpdateActionPointTextPopUp(_actionPointText);
                movementPlayer++;
                yield return _timeBetweenPlayerMovement;
            }
        }

        player.playerActionPoint = _actionPointText;
        ClearFollowPath(player);
    }

    private void UpdateActionPointTextPopUp(int actionPoint)
    {
        UiManager.Instance.textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(actionPoint);
        GameManager.Instance.currentPlayerTurn.playerActionPoint = UiManager.Instance.totalCurrentActionPoint;
    }

    public void SetFalsePathObjects()
    {
        foreach (var obj in pathObjects)
        {
            obj.SetActive(false);
        }
        pathObjects.Clear();
    }

    public void ResetPreviewPath(PlayerStateManager player)
    {
        foreach (var path in previewPath)
        {
            path.gameObject.SetActive(false);
        }
        previewPath.Clear();
        player.finalPathFinding.Clear();
    }

    private void ClearFollowPath(PlayerStateManager player)
    {
        player.currentBlockPlayerOn = player.currentTouchBlock;
        
        var currentNodePlayerOn = player.currentBlockPlayerOn.GetComponent<Node>();
        currentNodePlayerOn.isActive = false;
        //We add the player to the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = currentNodePlayerOn.groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Add(player.gameObject.transform);

        var pMovementManager = player.playerMovementManager;
        pMovementManager.ghostPlayer.SetActive(false);

        //Foreach block in our finalpathfinding we reset the previous blocks at the end of the loop
        foreach (Transform t in player.finalPathFinding)
        {
            t.GetComponent<Node>().previousBlock = null;
        }

        player.finalPathFinding.Clear();
        player.walking = false;

        if (EndZoneManager.Instance != null)
        {
            EndZoneManager.Instance.CheckPlayersTeam();
            EndZoneManager.Instance.PlayersIsOnEndZone();
        }

        SetFalsePathObjects();
    
        if (player.playerActionPoint > 0)
        {
            EnterState(player);
        }

        if (player.playerActionPoint <= 0)
        {
            currentNodePlayerOn.isActive = false;
            UiManager.Instance.buttonNextTurn.SetActive(true);
        }
    }
}