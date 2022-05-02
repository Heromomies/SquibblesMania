using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
    
    [SerializeField] private Image hat;

    
    public int hatID;

    [SerializeField] private Sprite[] hats;

    public GameObject otherPlayer;
    


    void Start()
    {

        

        SetItem("hats");

        
    }


    

    public void SelectHat(bool isForward)
    {
        if (isForward)
        {
            if (hatID == hats.Length - 1 || otherPlayer.GetComponent<CharacterCustomization>().hatID == hats.Length - 1 && hatID == hats.Length - 2)
            {
                hatID = 0;
            }
            else
            {
                hatID++;
            }

            if (hatID == otherPlayer.GetComponent<CharacterCustomization>().hatID)
            {
                hatID++;
            }

            
        }
        else
        {
            if (hatID == 0 || otherPlayer.GetComponent<CharacterCustomization>().hatID == 0 && hatID == 1)
            {
                hatID = hats.Length - 1;
            }
            else
            {
                hatID--;
            }

            if (hatID == otherPlayer.GetComponent<CharacterCustomization>().hatID)
            {
                hatID--;
            }
        }

        SetItem("hats");
    }

    private void SetItem(string type)
    {
        switch(type)
        {
            case "hats":
              hat.GetComponent<Image>().sprite = hats[hatID];
               
                break;
           

                
                
        }
    }

    

}
