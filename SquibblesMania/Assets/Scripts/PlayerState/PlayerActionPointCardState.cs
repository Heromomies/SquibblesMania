using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerActionPointCardState : PlayerBaseState
{
    public List<Transform> previewPath = new List<Transform>();
    [HideInInspector] public int actionPointText;
    [HideInInspector] public int actionPoint;
    
    //The state when player use is card action point
    public override void EnterState(PlayerStateManager player)
    {
        //TODO Faire une method qui permet de prévisualiser jusqu'ou le joueur peut aller avec ces points d'actions
        previewPath.Clear();
        PreviewPath(player.playerActionPoint, player);
    }

    #region PREVIEW

    private void PreviewPath(int actionPoint, PlayerStateManager player)
    {
        List<Transform> possiblePath = new List<Transform>();
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> finalPreviewPath = new List<Transform>();

        int indexBlockNearby = 0;

        //Take the base color of the block
        if (player.currentBlockPlayerOn != null)
        {
            Color blockBaseColor = Color.gray;
            player.currentBlockPlayerOn.gameObject.GetComponent<Renderer>().material.color = blockBaseColor;
            TouchManager.Instance.blockCurrentlySelectedColor = blockBaseColor;
        }


        //Foreach possible path compared to the block wich player is currently on
        foreach (GamePath path in player.currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            if (path.isActive)
            {
                possiblePath.Add(path.nextPath);
                finalPreviewPath.Add(path.nextPath);
                path.nextPath.GetComponent<Node>().previousBlock = player.currentBlockPlayerOn;
            }
        }


        //We add in our list of past blocks, the block which the player is currently on
        pastBlocks.Add(player.currentBlockPlayerOn);

        ExplorePreviewPath(possiblePath, pastBlocks, finalPreviewPath, indexBlockNearby, actionPoint, player);
    }

    private void ExplorePreviewPath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, int indexBlockNearby, int actionPoint, PlayerStateManager playerStateManager)
    {
        playerStateManager.nextBlockPath = finalPreviewPath;

        indexBlockNearby++;


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

        //If our current block is == to the player selected block then out of the loop
        if (indexBlockNearby == actionPoint)
        {
            ColorPossiblePaths(finalPreviewPath, Color.white);
            return;
        }

        CheckPossiblePaths(currentCheckedBlocks, previousBlocksPath, finalPreviewPath, nextBlocksPath);

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

    void ColorPossiblePaths(List<Transform> finalPreviewPath, Color color)
    {
        //Player have a preview of his possible movement
        foreach (var block in finalPreviewPath)
        {
            block.gameObject.GetComponent<Renderer>().material.color = color;
        }

        previewPath = finalPreviewPath;
        
    }

    void CheckPossiblePaths(List<Transform> currentCheckedBlocks, List<Transform> previousBlocksPath,
        List<Transform> finalPreviewPath, List<Transform> nextBlocksPath)
    {
        //Foreach currents checked block in our list
        foreach (Transform checkedBlock in currentCheckedBlocks)
        {
            //Foreach possible path in our currentCheckedBlock
            foreach (GamePath path in checkedBlock.GetComponent<Node>().possiblePath)
            {
                //We look if in our list of previousBlockPath, she's not already contains the next block and if the next block is active
                if (!previousBlocksPath.Contains(path.nextPath) && path.isActive)
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

    public override void UpdtateState(PlayerStateManager player)
    {
        //Update the preview Path of the player 
        if (GameManager.Instance.isPathRefresh && player.playerActionPoint > 0)
        {
            GameManager.Instance.isPathRefresh = false;
            ColorPossiblePaths(previewPath, Color.grey);
            EnterState(player);
            
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.isPlayerInActionCardState = false;
        ColorPossiblePaths(player.finalPathFinding, Color.grey);
        //Switch to next player of another team to play
        switch (player.playerNumber)
        {
            case 0:
                GameManager.Instance.ChangePlayerTurn(1);

                break;
            case 1:
                GameManager.Instance.ChangePlayerTurn(2);

                break;
            case 2:
                GameManager.Instance.ChangePlayerTurn(3);

                break;
            case 3:
                GameManager.Instance.ChangePlayerTurn(0);
                break;
        }
    }

    public void FindPath(PlayerStateManager player)
    {
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> nextBlocks = new List<Transform>();

        //Foreach possible path compared to the block wich player is currently on

        foreach (GamePath path in player.currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            //If we have a path who is activated then we add it to the list of nextBlockPath
            if (path.isActive)
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
            //We look if in our list of previousBlockPath, she's not already contains the next block and if the next block is active
            if (!previousBlocksPath.Contains(path.nextPath) && path.isActive)
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

        actionPointText = player.playerActionPoint;

        for (int i = player.finalPathFinding.Count - 1; i > 0; i--)
        {
            if (movementPlayer < player.playerActionPoint)
            {
                EventManager.Instance.AddPoint(1);
                Vector3 movePos = player.finalPathFinding[i].GetComponent<Node>().GetWalkPoint() +
                                  new Vector3(0, player.gameObject.transform.localScale.y / 2f, 0);
                player.transform.DOMove(movePos, player.timeMoveSpeed);
                player.finalPathFinding.Remove(player.finalPathFinding[i]);
                actionPointText--;
                UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(actionPointText);
                movementPlayer++;
                yield return new WaitForSeconds(0.4f);
            }
        }

        player.playerActionPoint = actionPointText;
        Clear(player);
    }

    private void Clear(PlayerStateManager player)
    {
        //We add the player to the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = player.currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Add(player.gameObject.transform);

        //Foreach block in our finalpathfinding we reset the previous blocks at the end of the loop
        foreach (Transform t in player.finalPathFinding)
        {
            t.GetComponent<Node>().previousBlock = null;
        }

        foreach (var previewBlock in previewPath)
        {
            previewBlock.gameObject.GetComponent<Renderer>().material.color = Color.grey;
        }

        player.finalPathFinding.Clear();
        player.walking = false;


        if (player.playerActionPoint > 0)
        {
            EnterState(player);
        }
        else
        {
            ExitState(player);
        }
    }
}