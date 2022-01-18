﻿//****************************************************************************
// Description:download newest version from: https://github.com/hiramtan/HiDebug_unity/releases
// Author: hiramtan@live.com
//****************************************************************************

using UnityEngine;

public class Example3 : MonoBehaviour
{
    [SerializeField]
    private bool _isLogOnText;
    [SerializeField]
    private bool _isLogOnScreen;
    // Use this for initialization
    void Start()
    {
        Debuger.EnableOnText(_isLogOnText);
        Debuger.EnableOnScreen(_isLogOnScreen);
    }
}
