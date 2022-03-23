using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSave : MonoBehaviour
{
    public GameObject P1;
    public GameObject P2;
    public GameObject P3;
    public GameObject P4;

    public PlayerData playerData;

    public int mapID;

    public void SaveData()
    {
        playerData.P1colorID = P1.GetComponent<CharacterCustomization>().colorID;
        playerData.P1hatID = P1.GetComponent<CharacterCustomization>().colorID;

        playerData.P2colorID = P2.GetComponent<CharacterCustomization>().colorID;
        playerData.P2hatID = P2.GetComponent<CharacterCustomization>().colorID;

        playerData.P3colorID = P3.GetComponent<CharacterCustomization>().colorID;
        playerData.P3hatID = P3.GetComponent<CharacterCustomization>().colorID;

        playerData.P4colorID = P4.GetComponent<CharacterCustomization>().colorID;
        playerData.P4hatID = P4.GetComponent<CharacterCustomization>().colorID;

        mapID = playerData.MapID;

        SceneManager.LoadScene(mapID);
    }

    
        
    
}
