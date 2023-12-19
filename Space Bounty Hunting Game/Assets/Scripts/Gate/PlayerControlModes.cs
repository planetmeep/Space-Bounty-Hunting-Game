using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class PlayerControlModes : MonoBehaviour
{

    public static PlayerControlModes instance = null;

    public GameObject playerGround;
    public GameObject playerShip;

    public CinemachineVirtualCamera virtualCamera;
    public Camera playerCamera;
    public float groundCameraSize;
    public float shipCameraSize;
    public bool manningShip;

    private void Awake()
    {
        SwitchToShip();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchToGround(Vector3 spawnPoint, Vector3 shipPausePoint) 
    {
        manningShip = false;
        playerGround.transform.position = spawnPoint;
        playerGround.SetActive(true);
        virtualCamera.Follow = playerGround.transform;
        playerCamera.orthographicSize = groundCameraSize;
        playerShip.transform.position = shipPausePoint;
        playerShip.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        playerShip.transform.rotation = Quaternion.Euler(0, 0, -180);
        playerShip.GetComponent<ShipController>().DisableControls();
    }

    public void SwitchToShip() 
    {
        manningShip = true;
        playerGround.SetActive(false);
        virtualCamera.Follow = playerShip.transform;
        playerCamera.orthographicSize = shipCameraSize;
        playerShip.GetComponent<ShipController>().controlsActive = true;
    }
}
