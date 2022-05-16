using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Image = UnityEngine.UI.Image;

public class SquipyAnimTween : MonoBehaviour
{

    public Sprite[] spriteSquipy;
    public Image imgSquipy;
    [SerializeField] private RectTransform rectTransform;
    void Start()
    {
        // Play a series of sprites on the window on repeat endlessly
        LeanTween.play(rectTransform, spriteSquipy).setLoopPingPong();
    }
    
    void Update()
    {
        
    }
}
