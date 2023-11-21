using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Different modes for Flying and Walking that assign delegates to the controls.
//Added unset so you always start unset and can pass in the mode you want at start.
public enum PlayerMode
{
     Unset,
    Flying,
    Walking
}

public class ControlsManager : MonoBehaviour
{
    //Mode for switching between flying and walking
    public PlayerMode currentMode = PlayerMode.Unset;
    // Reference to the input actions
}