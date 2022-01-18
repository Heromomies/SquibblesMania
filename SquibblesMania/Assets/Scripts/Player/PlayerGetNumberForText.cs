using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGetNumberForText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int number = GetComponentInParent<PlayerStateManager>().playerNumber + 1;
        GetComponent<TextMesh>().text = number.ToString();
    }
    
}
