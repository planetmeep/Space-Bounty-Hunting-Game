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
    // Delegate and Field for Mouse Position
    public Action<Vector2> onReceiveMousePosition;
    private Vector2 currentMousePosition;
  
 
   

    private void Awake()
    {
        // Initialize the input controls
        controls = new PlayerControls();

        // Link the input actions to methods
        controls.Gameplay.MoveForward.started += ctx => onStartThrustForward?.Invoke();
        controls.Gameplay.MoveForward.canceled += ctx => onStopThrustForward?.Invoke();
        controls.Gameplay.Backwards.started += ctx => onStartThrustBackward?.Invoke();
        controls.Gameplay.Backwards.canceled += ctx => onStopThrustBackward?.Invoke();
        // Set up the MousePosition action
        controls.Gameplay.MousePosition.performed += ctx => ReceiveMousePosition(ctx.ReadValue<Vector2>());

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
        onStartThrustForward -= shipController.StartThrustForward;
        onStopThrustForward -= shipController.StopThrustForward;
        onStartThrustBackward -= shipController.StartBraking;
        onStopThrustBackward -= shipController.StopBraking;
        onReceiveMousePosition -= shipController.UpdateMousePosition;
        

        currentMode = mode;

        switch (mode)
        {
            case PlayerMode.Flying:

            onStartThrustForward += shipController.StartThrustForward;
            onStopThrustForward += shipController.StopThrustForward;
            onStartThrustBackward += shipController.StartBraking;
            onStopThrustBackward += shipController.StopBraking;
           
           onReceiveMousePosition += shipController.UpdateMousePosition;
            break;

            case PlayerMode.Walking:
                
                break;
        }
    }
     //Sets and invokes the mouse position action
    private void ReceiveMousePosition(Vector2 mousePosition)
    {
        currentMousePosition = mousePosition;
        onReceiveMousePosition?.Invoke(currentMousePosition);
    }
    
    //Get the mouse position directly instead of through the delegate
    public Vector2 GetMousePosition()
    {
        return currentMousePosition;
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