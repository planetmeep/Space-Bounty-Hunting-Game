using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStationGate : MonoBehaviour
{
    public GameObject enterSpawnPoint;
    public GameObject exitSpawnPoint;
    public TractorBeam tractorBeam;
    public ExitGroup exitGroup;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (tractorBeam.playerColliding && PlayerControlModes.instance.manningShip) 
            {
                PlayerControlModes.instance.SwitchToGround(enterSpawnPoint.transform.position, exitSpawnPoint.transform.position);
            }
            else if (exitGroup.playerColliding) 
            {
                PlayerControlModes.instance.SwitchToShip();
            }
        }
    }
}
