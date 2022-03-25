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

    private Dictionary<string, string> colors =
        new Dictionary<string, string>()
        {
            {"Black", "#1A1A1A" },
            {"Blue", "#4C3CE7" },
            {"Green", "#008000" },
            {"Orange", "#E78A3C" },
            {"Pink", "#FD6C9E" },
            {"Red", "#E74C3C" },
            {"Violet", "#7A3CE7" },
            {"White", "#FFFFFF" },
            {"Yellow", "#FFFF00" }, 
        };

     public List<Material> colorstest = new List<Material>();
    public List<string> hex = new List<string>();

    public int colorID;
    public int hatID;

    [SerializeField] private Sprite[] hats;


    void Start()
    {

        for (int i = 0; i < colorstest.Count; i++)
        {
            hex.Add(ColorUtility.ToHtmlStringRGB(colorstest[i].color));
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
            if (colorID == colors.Count - 1)
            {
                colorID = 0;
            }
            else
            {
                colorID++;
            }
        }
        else
        {
            if (colorID == 0)
            {
                colorID = colors.Count - 1;
            }
            else
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
            if (hatID == hats.Length - 1)
            {
                hatID = 0;
            }
            else
            {
                hatID++;
            }
        }
        else
        {
            if (hatID == 0)
            {
                hatID = hats.Length - 1;
            }
            else
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
                hat.GetComponent<SpriteRenderer>().sprite = hats[hatID];
               
                break;
            case "colors":
                if (ColorUtility.TryParseHtmlString(hex[colorID], out Color color))
                {
                    bodycolor.GetComponent<Image>().color = color;
                    hat.GetComponent<Image>().color = color;
                    Debug.Log("lepain");

                }
                


                    break;

                
        }
    }

    

}
