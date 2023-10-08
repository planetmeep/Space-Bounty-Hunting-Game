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
    public Action onStartThrustForward;
    public Action onStopThrustForward;
    public Action onStartThrustBackward;
    public Action onStopThrustBackward;
    public Action onStartRotateLeft;
    public Action onStopRotateLeft;
    public Action onStartRotateRight;
    public Action onStopRotateRight;
   

    private void Awake()
{
    // Initialize the input controls
    controls = new PlayerControls();

    // Link the input actions to methods
    controls.Gameplay.MoveForward.started += ctx => onStartThrustForward?.Invoke();
    controls.Gameplay.MoveForward.canceled += ctx => onStopThrustForward?.Invoke();
    controls.Gameplay.Backwards.started += ctx => onStartThrustBackward?.Invoke();
    controls.Gameplay.Backwards.canceled += ctx => onStopThrustBackward?.Invoke();
    
    controls.Gameplay.TurnLeft.started += ctx => onStartRotateLeft?.Invoke();
    controls.Gameplay.TurnLeft.canceled += ctx => onStopRotateLeft?.Invoke();
    controls.Gameplay.TurnRight.started += ctx => onStartRotateRight?.Invoke();
    controls.Gameplay.TurnRight.canceled += ctx => onStopRotateRight?.Invoke();

    SetMode(PlayerMode.Flying);  // Ensure actions are hooked up at start
}
    public void SetMode(PlayerMode mode)
    {
        if (currentMode == mode)
        {
             Debug.Log("Exiting early since mode is the same.");
            return;
        }

        // Unsubscribe from all actions
        
        onStartRotateLeft -= shipController.StartRotateLeft;
        onStopRotateLeft -= shipController.StopRotateLeft;
        onStartRotateRight -= shipController.StartRotateRight;
        onStopRotateRight -= shipController.StopRotateRight;
        
        onStartThrustForward -= shipController.StartThrustForward;
        onStopThrustForward -= shipController.StopThrustForward;
        onStartThrustBackward -= shipController.StartThrustBackward;
        onStopThrustBackward -= shipController.StopThrustBackward;
        

        currentMode = mode;

        switch (mode)
        {
            case PlayerMode.Flying:
           
            onStartRotateLeft += shipController.StartRotateLeft;
            onStopRotateLeft += shipController.StopRotateLeft;
            onStartRotateRight += shipController.StartRotateRight;
            onStopRotateRight += shipController.StopRotateRight;

            onStartThrustForward += shipController.StartThrustForward;
            onStopThrustForward += shipController.StopThrustForward;
            onStartThrustBackward += shipController.StartThrustBackward;
            onStopThrustBackward += shipController.StopThrustBackward;
           
           
            break;

            case PlayerMode.Walking:
                
                break;
        }
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