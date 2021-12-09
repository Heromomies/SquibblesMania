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
        //Pour chaque path possible par rapport au block ou le joueur se trouve actuellement

        foreach (GamePath path in currentBlockPlayerOn.GetComponent<Node>().possiblePath)
        {
            //Si on a un path qui est actif alors on l'ajoute a notre list de nextBlocksPath
            if (path.isActive)
            {
                nextBlocks.Add(path.nextPath);
                //Dans notre element path on assigne notre previous block a notre block sur lequel le joueur se trouve actuellement
                path.nextPath.GetComponent<Node>().previousBlock = currentBlockPlayerOn;
            }
        }

        //On ajoute dans notre list des anciens blocks, le block où le joueur est actuellement dessus
        pastBlocks.Add(currentBlockPlayerOn);
        //On explore ensuite notre pathfinding
        ExplorePath(nextBlocks, pastBlocks);
        BuildPath();
    }

    private void ExplorePath(List<Transform> nextBlocksPath, List<Transform> previousBlocksPath)
    {
        //Le block ou le joueur est actuellement
        Transform currentBlock = nextBlocksPath[0];

        nextBlocksPath.Remove(currentBlock);
        //Si notre current block est = au block que le joueur a sélectionner alors on sors de la boucle
        if (currentBlock == currentTouchBlock)
        {
            return;
        }

        //Pour chaque passage possible dans notre currentBlock
        foreach (GamePath path in currentBlock.GetComponent<Node>().possiblePath)
        {
            //On regarde si dans notre list de previousBlockPath, elle ne contient pas déjà le prochain block (path.blcokPath) et si celui si est actif
            if (!previousBlocksPath.Contains(path.nextPath) && path.isActive)
            {
                //On ajoute dans notre list de nextPath ce prochain block
                nextBlocksPath.Add(path.nextPath);
                //On assigne ducoup le previous block a notre block actuel
                path.nextPath.GetComponent<Node>().previousBlock = currentBlock;
            }
        }

        //On ajoute dans notre list de path déjà visité notre currentBlock
        previousBlocksPath.Add(currentBlock);

        //Si dans notre list il reste un element on redemarre la void
        if (nextBlocksPath.Any())
        {
            ExplorePath(nextBlocksPath, previousBlocksPath);
        }
    }

    private void BuildPath()
    {
        //Le block actuellement selectionner par le joueur
        Transform block = currentTouchBlock;

        //Tant que le block sélectionner par le joueur n'est pas égale au block ou le joueur est censer etre 
        while (block != currentBlockPlayerOn)
        {
            //On ajoute ce block a notre list pathfinding final
            finalPathFinding.Add(block);

            //Si dans notre block (le block selectionner par le joueur) le block précedant n'est pas nul alors le block devient le préviousBlock
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

    //Movement du joueur
    private IEnumerator FollowPath()
    {
        for (int i = finalPathFinding.Count - 1; i > 0; i--)
        {
            Vector3 movePos = finalPathFinding[i].GetComponent<Node>().GetWalkPoint() + new Vector3(0, 0.5f, 0);
            transform.DOMove(movePos, timeMoveSpeed);
            yield return new WaitForSeconds(0.4f);
        }

        Clear();
    }

    private void Clear()
    {
        //Pour chaque cube dans notre pathfinding on remet reset les previous block a la fin du calcul
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up);
    }
}