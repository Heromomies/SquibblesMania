using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCustomization : MonoBehaviour
{
    [SerializeField] private Image bodycolor;

    public int colorID;

    public List<Material> colors = new List<Material>();
    public List<string> hex = new List<string>();

    public GameObject otherTeam;
    // Start is called before the first frame update

    void Start()
    {

        for (int i = 0; i < colors.Count; i++)
        {
            hex.Add(ColorUtility.ToHtmlStringRGB(colors[i].color));
        }

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
                if (ColorUtility.TryParseHtmlString("#" + hex[colorID], out Color color))
                {
                    bodycolor.GetComponent<Image>().color = color;
                    //hat.GetComponent<Image>().color = color;


                }

                break;



        }
    }


}
