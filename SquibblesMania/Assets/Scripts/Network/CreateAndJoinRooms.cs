using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    public List<string> userId = new List<string>();
    
    // Start is called before the first frame update
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoom.text);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public override void OnJoinedRoom()
    {
       /* Debug.Log("UserId=" + PhotonNetwork.AuthValues.UserId);
        userId.Add(PhotonNetwork.AuthValues.UserId);*/
        
        PhotonNetwork.LoadLevel("Game");
    }
}
