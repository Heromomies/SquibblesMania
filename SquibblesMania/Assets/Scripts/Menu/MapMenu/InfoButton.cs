using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        transform.localScale = Vector2.zero;
    }

    public void Open()
    {
        transform.LeanScale(Vector2.one, 0.3f);
    }

    public void Close()
    {
        transform.LeanScale(Vector2.zero, 0.8f).setEaseInBack(); 
    }
}
