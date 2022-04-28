using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] private Image bodycolor;
    [SerializeField] private Image hat;


    public List<Material> colors = new List<Material>();
    public List<string> hex = new List<string>();

    public int colorID;
    public int hatID;

    [SerializeField] private Sprite[] hats;

    public GameObject otherPlayer;
    public GameObject otherTeam;


    void Start()
    {

        for (int i = 0; i < colors.Count; i++)
        {
            hex.Add(ColorUtility.ToHtmlStringRGB(colors[i].color));
        }

        SetItem("colors");
        SetItem("hats");

        
    }

    private void Update()
    {

      
    }


    public void SelectColor(bool isForward)
    {
        if (isForward)
        {
            if (colorID == colors.Count - 1 || otherTeam.GetComponent<CharacterCustomization>().colorID == colors.Count - 1 && colorID == colors.Count - 2)
            {
                colorID = 0;
            }
            else
            {
                colorID++;
            }

            if (colorID == otherTeam.GetComponent<CharacterCustomization>().colorID)
            {
                colorID++;
            }


        }
        else
        {
            if (colorID == 0 || otherTeam.GetComponent<CharacterCustomization>().colorID == 0 && colorID == 1)
            {
                colorID = colors.Count - 1;
            }
            else
            {
                colorID--;
            }

            if (colorID == otherTeam.GetComponent<CharacterCustomization>().colorID)
            {
                colorID--;
            }

        }

        SetItem("colors");
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
            case "colors":
                if (ColorUtility.TryParseHtmlString("#" + hex[colorID], out Color color))
                {
                    bodycolor.GetComponent<Image>().color = color;
                    //hat.GetComponent<Image>().color = color;
                    

                }

                break;

                
                
        }
    }

    

}
