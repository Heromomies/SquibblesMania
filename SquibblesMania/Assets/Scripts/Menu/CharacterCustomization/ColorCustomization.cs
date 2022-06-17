using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCustomization : MonoBehaviour
{
    public Image teamBodyColor;
    [SerializeField] private Image memberOne;
    [SerializeField] private Image memberTwo;

    public int colorID;

    public List<Sprite> colors = new List<Sprite>();
    
    [SerializeField]
    private ColorCustomization otherTeam;
    // Start is called before the first frame update

    void Start()
    {
        SetItem("colors");
    }

    public void SelectColor(bool isForward)
    {
        AudioManager.Instance.Play("UI_Button_Other");
       
        if (isForward)
        {
            if (colorID == colors.Count - 1 || otherTeam.colorID == colors.Count - 1 && colorID == colors.Count - 2)
            {
                colorID = 0;
            }
            else
            {
                colorID++;
            }

            if (colorID == otherTeam.colorID)
            {
                colorID++;
            }



        }
        else
        {
            if (colorID == 0 || otherTeam.colorID == 0 && colorID == 1)
            {
                colorID = colors.Count - 1;
            }
            else
            {
                colorID--;
            }

            if (colorID == otherTeam.colorID)
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
                

              teamBodyColor.sprite = colors[colorID];
              memberOne.sprite = colors[colorID];
              memberTwo.sprite = colors[colorID];
                
                break;
        }

    }


}
