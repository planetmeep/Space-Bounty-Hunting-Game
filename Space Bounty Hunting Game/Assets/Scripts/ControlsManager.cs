using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    // Reference to the input actions
    private PlayerControls controls;

    private void Awake()
    {
        // Initialize the input controls
        controls = new PlayerControls();

        // Link the input actions to methods
        controls.Gameplay.MoveForward.performed += ctx => MoveForward();
        controls.Gameplay.TurnLeft.performed += ctx => TurnLeft();
        controls.Gameplay.Backwards.performed += ctx => TurnBackwards();
        controls.Gameplay.TurnRight.performed += ctx => TurnRight();
    }

    // Methods called when each action is performed

    private void MoveForward()
    {
        Debug.Log("Moving forward.");
    }

    private void TurnLeft()
    {
        Debug.Log("Turning left.");
    }

    private void TurnBackwards()
    {
        Debug.Log("Moving backwards.");
    }

    private void TurnRight()
    {
        Debug.Log("Turning right.");
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