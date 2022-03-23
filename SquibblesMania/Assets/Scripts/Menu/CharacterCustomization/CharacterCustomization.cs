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
            {"Red", "#E74C3C" },
            {"Yellow", "#FFFF00" },
            {"Green", "#008000" },
            {"Blue", "#4C3CE7" },
            {"Violet", "#7A3CE7" },
            {"Orange", "#E78A3C" },
            {"Pink", "#FD6C9E" },
            {"Black", "#1A1A1A" }
        };

    public int colorID;
    public int hatID;

    [SerializeField] private Sprite[] hats;


    void Start()
    {
        SetItem("colors");
        SetItem("hats");
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
                if(ColorUtility.TryParseHtmlString(colors.Values.ElementAt(colorID), out Color color)){
                    bodycolor.GetComponent<Image>().color = color;
                    hat.GetComponent<Image>().color = color;
                }
                break;
        }
    }

    

}
