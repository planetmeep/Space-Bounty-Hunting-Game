using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FighterController : MonoBehaviour, IKillable
{
    public bool roamer;
    public FighterWeaponController weaponController;
    public EnemyShipController shipController;
    public Rigidbody2D rb;
    public float viewRaycastDistance;
    public float obstacleRaycastDistance;
    public float bulletRadius;
    public float aimTime;
    public int viewAngle;
    public LayerMask playerSightLayers;
    public LayerMask obstacleSightLayers;
    private Vector3 targetVelocity = Vector3.zero;
    private Vector3 lookVector;
    private Vector3 aimVector;
    private Vector3 aimVelocity = Vector3.zero;
    private Vector3 obstacleNormal = Vector3.zero;
    private Transform playerTransform;
    private States AIState;
    private bool seePlayer;
    private bool shotOnPlayer;
    private bool obstacleInWay;

    public enum States 
    {
        Idle,
        Pursuit,
        Attack,
        Scatter
    }

    // Start is called before the first frame update
    void Start()
    {
        lookVector = transform.up;
        seePlayer = false;
        shotOnPlayer = false;
        playerTransform = PlayerControlModes.instance.playerShip.transform;
        AIState = States.Pursuit;
    }

    // Update is called once per frame
    void Update()
    {
        seePlayer = SeePlayer(viewRaycastDistance, viewAngle, lookVector);
        //shotOnPlayer = seePlayer ? HasShotOnPlayer(viewRaycastDistance, lookVector) : false;
        //lookVector = (seePlayer) ? (playerTransform.transform.position - transform.position).normalized : lookVector;
        lookVector = (playerTransform.transform.position - transform.position).normalized;

        obstacleInWay = SeeObstacle(obstacleRaycastDistance);
        print(obstacleInWay);
        lookVector = obstacleInWay ? /*(obstacleNormal - transform.position).normalized*/ obstacleNormal.normalized : lookVector;

        switch (AIState)
        {
            case States.Idle:
                if (roamer)
                {

                }
                else
                {

                }
                break;
            case States.Pursuit:
                shipController.thrustForward = true;
                shipController.applyBrakes = false;

                if (seePlayer) 
                {
                    AIState = States.Attack;
                }
                break;
            case States.Attack:
                lookVector = (playerTransform.position - transform.position).normalized;
                shipController.thrustForward = false;
                shipController.applyBrakes = true;
                if (!seePlayer) 
                {
                    AIState = States.Pursuit;
                }
                break;
            case States.Scatter:
                shipController.thrustForward = true;
                break;
        }

        SmoothAim();
    }

    private bool SeePlayer(float distance, int angle, Vector3 direction)
    {

        for (int i = 0; i < angle; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (transform.up), distance, playerSightLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up), distance, playerSightLayers);
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (transform.up) * viewRaycastDistance);
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up) * viewRaycastDistance);
            if (upperHit && (upperHit.collider.CompareTag("PlayerShip"))) return true;
            if (lowerHit && (lowerHit.collider.CompareTag("PlayerShip"))) return true;
        }

        return false;
    }


    private bool SeeObstacle(float distance)
    {

        for (int i = 0; i < 90; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (transform.up), distance, obstacleSightLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up), distance, obstacleSightLayers);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (transform.up) * obstacleRaycastDistance);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up) * obstacleRaycastDistance);
            if (upperHit)
            {
                obstacleNormal = Quaternion.Euler(0, 0, i + 90) * (transform.up);
                //obstacleNormal = upperHit.normal;

                return true;
            }
            if (lowerHit)
            {
                obstacleNormal = Quaternion.Euler(0, 0, -i + 90) * (transform.up);
                //obstacleNormal = lowerHit.normal;
                return true;
            }
        }

        obstacleNormal = Vector3.zero;
        return false;
    }


    private bool HasShotOnPlayer(float distance, Vector3 direction)
    {
        RaycastHit2D circlehit = Physics2D.CircleCast(transform.position, bulletRadius, (playerTransform.position - transform.position).normalized, distance, playerSightLayers);
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, distance, playerSightLayers);

        Debug.DrawRay(transform.position, (playerTransform.position - transform.position).normalized, Color.green);


        if (!rayhit || (!rayhit.collider.CompareTag("Player")))
        {
            return false;
        }

        if (circlehit && (circlehit.collider.CompareTag("Player")))
        {
            return true;
        }

        return false;
    }



    private void Scatter() 
    {

    }


    public void SmoothAim()
    {
        if (aimVector != lookVector)
        {
            aimVector = Vector3.SmoothDamp(aimVector, lookVector, ref aimVelocity, aimTime);
        }
        weaponController.SetLookVector(aimVector);
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }
}
