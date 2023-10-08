using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private PlayerControls controls;
    [SerializeField]
    private ShipController shipController;

    //Action Delegate
    public Action onMoveForward;
    public Action onTurnLeft;
    public Action onTurnRight;
    public Action onBackwards;

    private void Awake()
    {
        // Initialize the input controls
        controls = new PlayerControls();

        // Link the input actions to methods
        controls.Gameplay.MoveForward.performed += ctx => MoveForward();
        controls.Gameplay.TurnLeft.performed += ctx => TurnLeft();
        controls.Gameplay.Backwards.performed += ctx => Backwards();
        controls.Gameplay.TurnRight.performed += ctx => TurnRight();

        SetMode(PlayerMode.Flying);  // Ensure actions are hooked up at start
    }
    public void SetMode(PlayerMode mode)
    {
        Debug.Log("ShipController is: " + (shipController == null ? "NULL" : "Not NULL"));
        Debug.Log("Mode is: " + " current mode is: " + currentMode);

        if (currentMode == mode)
        {
             Debug.Log("Exiting early since mode is the same.");
            return;
        }

        // Unsubscribe from all actions
        onMoveForward -= shipController.MoveForward;
        onTurnLeft -= shipController.TurnLeft;
        onTurnRight -= shipController.TurnRight;
        onBackwards -= shipController.Backwards;
        

        currentMode = mode;

        switch (mode)
        {
            case PlayerMode.Flying:
                onMoveForward += shipController.MoveForward;
                onTurnLeft += shipController.TurnLeft;
                onTurnRight += shipController.TurnRight;
                onBackwards += shipController.Backwards;
                Debug.Log("Subscribed to ShipController methods");
                break;

            case PlayerMode.Walking:
                
                break;
        }
    }

    // Methods called when each action is performed

    private void MoveForward()
    {
        Debug.Log("ControlsManager: MoveForward Detected");
        onMoveForward?.Invoke();
    }

    private void TurnLeft()
    {
        onTurnLeft?.Invoke();
    }

    private void Backwards()
    {
        onBackwards?.Invoke();
    }

    private void TurnRight()
    {
        onTurnRight?.Invoke();
    }

    private void OnEnable()
    {
        // Enable the Gameplay action map
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        // Disable the Gameplay action map
        controls.Gameplay.Disable();
    }
}