using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSave : MonoBehaviour
{
    [SerializeField]
    private CharacterCustomization player1;
    [SerializeField]
    private CharacterCustomization player2;
    [SerializeField]
    private CharacterCustomization player3;
    [SerializeField]
    private CharacterCustomization player4;
    [SerializeField]
    private ColorCustomization team1;
    [SerializeField]
    private ColorCustomization team2;

    public PlayerData playerData;

    public int mapID;

    public void SaveData()
    {
        playerData.P1colorID = team1.colorID;
        playerData.P1hatID = player1.hatID;

        playerData.P2colorID = team2.colorID;
        playerData.P2hatID = player2.hatID;

        playerData.P3colorID = team1.colorID;
        playerData.P3hatID = player3.hatID;

        playerData.P4colorID = team2.colorID;
        playerData.P4hatID = player4.hatID;

        mapID = playerData.MapID;
        
        LoadSceneManager.Instance.LoadScene(mapID);
    }

    
        
    
}
