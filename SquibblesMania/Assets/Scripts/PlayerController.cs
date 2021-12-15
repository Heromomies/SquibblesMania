using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviour
{
    public Transform currentBlockPlayerOn, currentTouchBlock;


    public List<Transform> finalPathFinding = new List<Transform>();
    public bool walking;
    public float timeMoveSpeed;

    private static PlayerController _playerController;
    public static PlayerController Instance => _playerController;

    private void Awake()
    {
        _playerController = this;
        DetectBlockBelowPlayer();
    }

    private void Start()
    {
        //Assign the player to a list for know on what block group is currently on

        if (currentBlockPlayerOn != null)
        {
            GroupBlockDetection groupBlockDetection = currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
            groupBlockDetection.playersOnGroupBlock.Add(gameObject.transform);
        }
    }


    private void Update()
    {
        DetectBlockBelowPlayer();
    }

    public void StartPathFinding()
    {
        finalPathFinding.Clear();
        FindPath();
    }

    private void FindPath()
    {
        List<Transform> pastBlocks = new List<Transform>();
        List<Transform> nextBlocks = new List<Transform>();

        //Foreach possible path compared to the block wich player is currently on

        foreach (GamePath path in currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            //If we have a path who is activated then we add it to the list of nextBlockPath
            if (path.isActive)
            {
                nextBlocks.Add(path.nextPath);
                //In our path element we assign our previous block to the block wich player is currently on
                path.nextPath.GetComponent<Node>().previousBlock = currentBlockPlayerOn;
            }
        }


        //We add in our list of past blocks, the block which the player is currently on
        pastBlocks.Add(currentBlockPlayerOn);
        //We explore our pathfinding
        ExplorePath(nextBlocks, pastBlocks);
        BuildPath();
    }

    private void ExplorePath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath)
    {
        //The block wich the player is currently on
        Transform currentBlock = nextBlocksPath[0];

        nextBlocksPath.Remove(currentBlock);
        //If our current block is = to the player selected block then out of the loop
        if (currentBlock == currentTouchBlock)
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
            ExplorePath(nextBlocksPath, previousBlocksPath);
        }
    }

    private void BuildPath()
    {
        //The block currently selectionned by the player
        Transform block = currentTouchBlock;

        //We remove the player from the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Remove(gameObject.transform);


        //While the player selected block is != to the block wich player supposed to be 
        while (block != currentBlockPlayerOn)
        {
            //We add this block to our list final pathfinding
            finalPathFinding.Add(block);

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

        finalPathFinding.Insert(0, currentTouchBlock);


        if (!walking)
        {
            walking = true;
            StartCoroutine(FollowPath());
        }
    }

    //Movement of player
    private IEnumerator FollowPath()
    {
        for (int i = finalPathFinding.Count - 1; i > 0; i--)
        {
            Vector3 movePos = finalPathFinding[i].GetComponent<Node>().GetWalkPoint() + new Vector3(0, gameObject.transform.localScale.y/2f, 0);
            transform.DOMove(movePos, timeMoveSpeed);
            yield return new WaitForSeconds(0.4f);
        }

        Clear();
    }

    private void Clear()
    {
        //We add the player to the list of block group which the player is currently on 
        GroupBlockDetection groupBlockDetection = currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Add(gameObject.transform);

        //Foreach block in our finalpathfinding we reset the previous blocks at the end of the loop
        foreach (Transform t in finalPathFinding)
        {
            t.GetComponent<Node>().previousBlock = null;
        }

        finalPathFinding.Clear();
        walking = false;
    }

    private void DetectBlockBelowPlayer()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockPlayerOn = hit.transform;
            }
        }
    }
}