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
    [SerializeField]
    private Crosshair crosshair;
    //Action Delegate
    public Action onStartThrustForward;
    public Action onStopThrustForward; 
    public Action onStartThrustBackward;
    public Action onStopThrustBackward;
    public Action onStartLeftThrust;
    public Action onStopLeftThrust;
    public Action onStartRightThrust;
    public Action onStopRightThrust;
    
    // Delegate and Field for Mouse Position
    public Action<Vector2> onReceiveMousePosition;
    private Vector2 currentMousePosition;

    public Action onShootStarted;
    public Action onShootStopped;
  
 
   

    private void Awake()
    {
        // Initialize the input controls
        controls = new PlayerControls();

        // Link the input actions to methods
        controls.Gameplay.MoveForward.started += ctx => onStartThrustForward?.Invoke();
        controls.Gameplay.MoveForward.canceled += ctx => onStopThrustForward?.Invoke();
        controls.Gameplay.Backwards.started += ctx => onStartThrustBackward?.Invoke();
        controls.Gameplay.Backwards.canceled += ctx => onStopThrustBackward?.Invoke();
        controls.Gameplay.Left.started += ctx => onStartLeftThrust?.Invoke();
        controls.Gameplay.Left.canceled += ctx => onStopLeftThrust?.Invoke();
        controls.Gameplay.Right.started += ctx => onStartRightThrust?.Invoke();
        controls.Gameplay.Right.canceled += ctx => onStopRightThrust?.Invoke();
   

        controls.Gameplay.Shoot.started += ctx => onShootStarted?.Invoke();
        controls.Gameplay.Shoot.canceled += ctx => onShootStopped?.Invoke();

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
        onReceiveMousePosition -= crosshair.UpdateCrosshairPosition;
        onShootStarted -= shipController.StartShooting;
        onShootStopped -= shipController.StopShooting;
        onStartRightThrust -= shipController.StartRightThrust;
        onStopRightThrust -= shipController.StopRightThrust;
        onStartLeftThrust -= shipController.StartLeftThrust;
        onStopLeftThrust -= shipController.StopLeftThrust;

        currentMode = mode;

        switch (mode)
        {
            case PlayerMode.Flying:

            onStartThrustForward += shipController.StartThrustForward;
            onStopThrustForward += shipController.StopThrustForward;
            onStartThrustBackward += shipController.StartBraking;
            onStopThrustBackward += shipController.StopBraking;
            onShootStarted += shipController.StartShooting;
            onShootStopped += shipController.StopShooting;
              onStartRightThrust += shipController.StartRightThrust;
        onStopRightThrust += shipController.StopRightThrust;
        onStartLeftThrust += shipController.StartLeftThrust;
        onStopLeftThrust += shipController.StopLeftThrust;
           
            onReceiveMousePosition += shipController.UpdateMousePosition;
            onReceiveMousePosition += crosshair.UpdateCrosshairPosition;
                break;

            case PlayerMode.Walking:
                
                break;
        }
    }
    private void Update()
    {
        // Continuously get the mouse position and invoke the associated actions
        Vector2 mousePos = Mouse.current.position.ReadValue();
        ReceiveMousePosition(mousePos);
        
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