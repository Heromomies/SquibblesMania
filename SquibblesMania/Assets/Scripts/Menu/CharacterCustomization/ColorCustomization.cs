using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCustomization : MonoBehaviour
{
    [SerializeField] private Image bodycolor;

    public int colorID;

    public List<Sprite> colors = new List<Sprite>();
    

    public GameObject otherTeam;
    // Start is called before the first frame update

    void Start()
    {
        SetItem("colors");
    }

    public void SelectColor(bool isForward)
    {
        if (isForward)
        {
            if (colorID == colors.Count - 1 || otherTeam.GetComponent<ColorCustomization>().colorID == colors.Count - 1 && colorID == colors.Count - 2)
            {
                colorID = 0;
            }
            else
            {
                colorID++;
            }

            if (colorID == otherTeam.GetComponent<ColorCustomization>().colorID)
            {
                colorID++;
            }



        }
        else
        {
            if (colorID == 0 || otherTeam.GetComponent<ColorCustomization>().colorID == 0 && colorID == 1)
            {
                colorID = colors.Count - 1;
            }
            else
            {
                colorID--;
            }

            if (colorID == otherTeam.GetComponent<ColorCustomization>().colorID)
            {
                colorID--;
            }

        }

        SetItem("colors");
    }

    private void SetItem(string type)
    {
        switch (type)
        {
            case "colors":
                
              bodycolor.GetComponent<Image>().sprite = colors[colorID];
                  
            break;
        }

    }


}
