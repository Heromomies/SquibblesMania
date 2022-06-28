using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class PlayerActionPointCardState : PlayerBaseState
{
    private int _actionPointText;
    private List<GameObject> pathObjects = new List<GameObject>();
    private List<Transform> previewPath = new List<Transform>();


    private WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.6f);


    //The state when player use is card action point
    public override void EnterState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = true;
        player.nextBlockPath.Clear();
        SetFalsePathObjects();
        ResetPreviewPath(player);
        player.currentBlocPlayerOn.GetComponent<Node>().isActive = true;
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
            if (player.currentBlocPlayerOn.TryGetComponent(out Node blocNode))
            {
                foreach (GamePath path in blocNode.possiblePath)
                {
                    Node actualNode = blocNode;
                    if (path.nextPath.TryGetComponent(out Node nextPathNode))
                    {
                        bool isNextPathActive = nextPathNode.isActive;

                        if (path.isActive && isNextPathActive && actualNode.isActive)
                        {
                            possiblePath.Add(path.nextPath);
                            finalPreviewPath.Add(path.nextPath);
                            nextPathNode.previousBlock = player.currentBlocPlayerOn;
                        }
                    }
                }
            }
            
            finalPreviewPath.Add(player.currentBlocPlayerOn);

            //We add in our list of past blocks, the block which the player is currently on
            pastBlocks.Add(player.currentBlocPlayerOn);

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

    void CheckPossiblePaths(List<Transform> currentCheckedBlocks, List<Transform> previousBlocksPath, List<Transform> finalPreviewPath, List<Transform> nextBlocksPath, PlayerStateManager player)
    {
        //Foreach currents checked block in our list
        foreach (Transform checkedBlock in currentCheckedBlocks)
        {
            if (checkedBlock.TryGetComponent(out Node blocNode))
            {
                foreach (var path in blocNode.possiblePath)
                {
                    var pathParentPos = path.nextPath.transform.parent.position;
                    var checkedBlockPos = checkedBlock.transform.position;

                    if (path.nextPath.TryGetComponent(out Node pathNode))
                    { 
                        var isNextPathActive = pathNode.isActive;
                        //We look if in our list of previousBlockPath, she's not already contains the next block and if the next block is active
                        if (!previousBlocksPath.Contains(path.nextPath) && path.isActive && isNextPathActive && PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position) &&
                            CurrentCheckedBlocPosComparedToNextBlocPos(checkedBlockPos, path.nextPath.transform.position) && !finalPreviewPath.Contains(path.nextPath))
                        {
                            //We add in our list the next block
                            nextBlocksPath.Add(path.nextPath);
                            finalPreviewPath.Add(path.nextPath);
                            //We assign the previous block to our currently block
                            pathNode.previousBlock = checkedBlock;
                        }
                    }
                    
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
        
        return pathParentPos.y + 1.5f - playerPos.y > -0.1f && pathParentPos.y + 1.5f - playerPos.y < 0.1f;
    }

    public override void UpdateState(PlayerStateManager player)
    {
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = false;
        player.indicatorPlayerRenderer.gameObject.SetActive(false);

        foreach (var obj in pathObjects)
        {
            obj.SetActive(false);
        }

        pathObjects.Clear();
        
        PlayerMovementManager.Instance.isPlayerPreviewPath = false;
        player.ResetPreviewPathFinding();
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

        if (player.currentBlocPlayerOn.TryGetComponent(out Node blocNode))
        {
            foreach (var gamePath in blocNode.possiblePath)
            {
                var pathParentPos = gamePath.nextPath.transform.parent.position;
                if (gamePath.nextPath.TryGetComponent(out Node gamePathNode))
                {
                    var isNextPathActive = gamePathNode.isActive;
                    if (gamePath.isActive && isNextPathActive && PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position))
                    {
                        nextBlocks.Add(gamePath.nextPath);
                        gamePathNode.previousBlock = player.currentBlocPlayerOn;
                    }
                }
            }
        }

        //We add in our list of past blocks, the block which the player is currently on
        pastBlocks.Add(player.currentBlocPlayerOn);
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
        if (currentBlock == player.currentTouchBloc)
        {
            //Player arrive to the destination
            return;
        }

        if (currentBlock.TryGetComponent(out Node currentBlocNode))
        {
            foreach (GamePath path in currentBlocNode.possiblePath)
            {
                var pathParentPos = path.nextPath.transform.parent.position;
                var isCurrentBlocNodeActive = currentBlocNode.isActive;
                
                if (!previousBlocksPath.Contains(path.nextPath) && path.isActive && isCurrentBlocNodeActive && PathParentPosComparedToPlayerPos(pathParentPos, player.transform.position))
                {
                    //We add in our list the next block
                    nextBlocksPath.Add(path.nextPath);

                    if (path.nextPath.TryGetComponent(out Node nextPathNode))
                    {
                        nextPathNode.previousBlock = currentBlock;
                    }
                }
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
        Transform block = player.currentTouchBloc;

        //While the player selected block is != to the block wich player supposed to be 
        while (block != player.currentBlocPlayerOn)
        {
            //We add this block to our list final pathfinding
            player.finalPathFinding.Add(block);

            
            //If in our selected block, the precedent block is not nul then the block become the past block
            if (block.TryGetComponent(out Node blocNode) && blocNode.previousBlock != null)
            {
                block = blocNode.previousBlock;
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

        player.finalPathFinding.Insert(0, player.currentTouchBloc);
    }

    private IEnumerator ShowPreviewPath(PlayerStateManager player)
    {
        var instanceGhostPlayer = PlayerMovementManager.Instance.ghostPlayer;
        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {
            if (player.finalPathFinding[i].TryGetComponent(out Node finalBlocNode))
            {
                var spawnBulletPoint = finalBlocNode.GetWalkPoint();
                var sphere = PoolManager.Instance.SpawnObjectFromPool("SphereShowPath", new Vector3(spawnBulletPoint.x, spawnBulletPoint.y + 1.2f, spawnBulletPoint.z), Quaternion.identity, null);
                previewPath.Add(sphere.transform);
            }
            
        }

        if (player.currentTouchBloc.TryGetComponent(out Node touchBlocNode))
        {
            var ghostPlayerPos = touchBlocNode.GetWalkPoint() + new Vector3(0, 0.5f, 0);
            instanceGhostPlayer.SetActive(true);
            instanceGhostPlayer.transform.position = ghostPlayerPos;
            previewPath.Add(instanceGhostPlayer.transform);
        }
        
        yield return null;
    }

    public IEnumerator BeginFollowPath(PlayerStateManager player)
    {
        UiManager.Instance.SpawnTextActionPointPopUp(player.transform);
        _actionPointText = player.playerActionPoint;
        yield return null;
        player.StartCoroutine(FollowPath(player));
    }

    //Movement of player
    private IEnumerator FollowPath(PlayerStateManager player)
    {
        
        //We remove the player from the list of block group which the player is currently on 
        if (player.currentBlocPlayerOn.TryGetComponent(out Node currentBlocNode))
        {
            GroupBlockDetection groupBlockDetection = currentBlocNode.groupBlockParent;
            groupBlockDetection.playersOnGroupBlock.Remove(player.gameObject.transform);
        }

        UiManager.Instance.sliderNextTurn.interactable = false;
      
        NFCManager.Instance.displacementActivated = true;
        player.playerAnimator.SetBool("isMoving", player.walking);
        
        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {

            if (player.finalPathFinding[i].TryGetComponent(out Node finalBlocNode))
            {
                var walkPoint = finalBlocNode.GetWalkPoint();

                var movePos = walkPoint + new Vector3(0, 1, 0);
                var direction = (movePos - player.transform.position).normalized;
                var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                
                player.transform.LeanMove(movePos, player.timeMoveSpeed);
                player.transform.DORotateQuaternion(Quaternion.Euler(0, targetAngle, 0), player.timeRotateSpeed);
                _actionPointText--;
            
                yield return _timeBetweenPlayerMovement;
                
                GameManager.Instance.PlayerMoving();
            }
            
          
        }
        UpdateActionPointTextPopUp(_actionPointText);
        player.playerActionPoint = _actionPointText;
        
        ClearFollowPath(player);
    }

    private void UpdateActionPointTextPopUp(int actionPoint)
    {
        if (UiManager.Instance.textActionPointPopUp.TryGetComponent(out PopUpTextActionPoint popUpTextActionPoint))
        {
            popUpTextActionPoint.SetUpText(actionPoint);
            GameManager.Instance.currentPlayerTurn.playerActionPoint = UiManager.Instance.totalCurrentActionPoint;
        }
       
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
        player.currentBlocPlayerOn = player.currentTouchBloc;
        
        GameManager.Instance.DetectParentBelowPlayers();

        var pMovementManager = player.playerMovementManager;
        pMovementManager.ghostPlayer.SetActive(false);
        UiManager.Instance.sliderNextTurn.interactable = true;
        //Foreach block in our finalpathfinding we reset the previous blocks at the end of the loop
        foreach (Transform finalBloc in player.finalPathFinding)
        {
            if (finalBloc.TryGetComponent(out Node finalBlocNode ))
            {
                finalBlocNode.previousBlock = null;
            }
            
        }

        if (player.playerActionPoint == 0)
        {
            AudioManager.Instance.Play("UI_EndTurn_Other");
            NFCController.StopPolling();
            LightController.ShutdownAllLights();
        }
        
        player.finalPathFinding.Clear();
        player.walking = false;
        player.playerAnimator.SetBool("isMoving", player.walking);
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
        
    }
}