using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour, IKillable
{
    public bool roamer;
    public FighterWeaponController weaponController;
    public EnemyShipController shipController;
    public Rigidbody2D rb;
    public float viewRaycastDistance;
    public float obstacleDetectionRadius;
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
    private CircleCollider2D circleCollider;
    private float colliderRadius;
    private Transform playerTransform;
    private States AIState;
    private bool seePlayer;
    private bool shotOnPlayer;
    private GameObject searchPoint;
    private Collider2D[] obstacleColliders;

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
        circleCollider = GetComponent<CircleCollider2D>();
        colliderRadius = circleCollider.radius;
        lookVector = transform.up;
        seePlayer = false;
        shotOnPlayer = false;
        playerTransform = PlayerControlModes.instance.playerShip.transform;
        searchPoint = new GameObject("SearchPoint");
        searchPoint.transform.position = playerTransform.position;
        AIState = States.Pursuit;
    }

    // Update is called once per frame
    void Update()
    {
        print(AIState);
        seePlayer = SeePlayer();
        if (seePlayer) searchPoint.transform.position = playerTransform.position;


        //shotOnPlayer = seePlayer ? HasShotOnPlayer(viewRaycastDistance, lookVector) : false;
        //lookVector = (seePlayer) ? (playerTransform.transform.position - transform.position).normalized : lookVector;
        //lookVector = (playerTransform.transform.position - transform.position).normalized;

        //obstacleInWay = SeeObstacle(obstacleRaycastDistance);

        switch (AIState)
        {
            case States.Idle:
                shipController.thrustForward = false;
                shipController.applyBrakes = true;


                if (seePlayer)
                {
                    AIState = States.Pursuit;
                }


                break;
            case States.Pursuit:
                shipController.thrustForward = true;
                shipController.applyBrakes = false;

                DetectObstacles();
                lookVector = GetPursuitDirection();
                //print(GetPursuitDirection());

                /*if (seePlayer) 
                {
                    AIState = States.Attack;
                }*/
                if (Vector3.Distance(searchPoint.transform.position, transform.position) < 0.5f)
                {
                    AIState = States.Idle;
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

    private bool SeePlayer()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, (playerTransform.position - transform.position).magnitude, playerSightLayers);
        /*for (int i = 0; i < angle; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (transform.up), distance, playerSightLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up), distance, playerSightLayers);
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (transform.up) * viewRaycastDistance);
            //Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up) * viewRaycastDistance);
            if (upperHit && (upperHit.collider.CompareTag("PlayerShip"))) return true;
            if (lowerHit && (lowerHit.collider.CompareTag("PlayerShip"))) return true;
        }*/

        if (hit && hit.collider.CompareTag("PlayerShip")) return true;
        return false;
    }

    /*private bool SeeObstacle(float distance)
    {

        for (int i = 0; i < 90; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (transform.up), distance, obstacleSightLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up), distance, obstacleSightLayers);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (transform.up) * obstacleRaycastDistance);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (transform.up) * obstacleRaycastDistance);

            if (upperHit)
            {


                turningRight = false;
                obstacleNormal = turningRight ? Quaternion.Euler(0, 0, i - 90) * (transform.up) : Quaternion.Euler(0, 0, i + 90) * (transform.up);
                return true;
            }
            if (lowerHit)
            {

                turningRight = true;
                obstacleNormal = turningRight ? Quaternion.Euler(0, 0, -i - 90) * (transform.up) : Quaternion.Euler(0, 0, -i + 90) * (transform.up);
                return true;
            }
        }

        obstacleNormal = Vector3.zero;
        return false;
    }*/


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
    private void DetectObstacles() 
    {
        obstacleColliders = Physics2D.OverlapCircleAll(transform.position, obstacleDetectionRadius, obstacleSightLayers);
    }

    private Vector3 GetPursuitDirection() 
    {
        float[] playerWeights = new float[8];
        float[] obstacleWeights = new float[8];

        //Obstacle Weights
        foreach (Collider2D collider in obstacleColliders) 
        {
            Vector2 dirToObstacle = collider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distObstacle = dirToObstacle.magnitude;
            float distanceWeight = distObstacle <= colliderRadius ? 1 : (obstacleDetectionRadius - distObstacle) / obstacleDetectionRadius;

            for (int i = 0; i < 8; i++) 
            {
                float dotProduct = Vector2.Dot(Directions.eightDirections[i], dirToObstacle.normalized);
                //dotProduct = Mathf.Clamp01(dotProduct);
                float finalWeight = dotProduct * distanceWeight;

                if (finalWeight > obstacleWeights[i]) 
                {
                    obstacleWeights[i] = finalWeight;
                }
            }
        }

        //Player Weights
        Vector2 dirToPlayer = (Vector2)searchPoint.transform.position - (Vector2)transform.position;
        for (int i = 0; i < 8; i++)
        {
            float dotProduct = Vector2.Dot(Directions.eightDirections[i], dirToPlayer.normalized);
            //dotProduct = Mathf.Clamp01(dotProduct);
            playerWeights[i] = dotProduct;
        }


        float[] finalWeights = new float[8];
        for (int i = 0; i < 8; i++) 
        {
            finalWeights[i] = playerWeights[i] - obstacleWeights[i];
            finalWeights[i] = Mathf.Clamp01(finalWeights[i]);
        }

        Vector2 targetDirection = Vector3.zero;
        float lastWeight = 0;
        for (int i = 0; i < 8; i++) 
        {
            if (finalWeights[i] > lastWeight) 
            {
                lastWeight = finalWeights[i];
                targetDirection = Directions.eightDirections[i] * finalWeights[i];
            }
            //targetDirection += Directions.eightDirections[i] * finalWeights[i];
        }
        targetDirection.Normalize();

        Debug.DrawRay(transform.position, targetDirection, Color.green);
        return new Vector3(targetDirection.x, targetDirection.y, transform.position.z);
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


public static class Directions
{
    public static List<Vector2> eightDirections = new List<Vector2>{
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
}



