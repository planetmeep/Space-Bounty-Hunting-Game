using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bodyguard : MonoBehaviour, IKillable
{
    public Rigidbody2D rb;
    public BodyguardWeaponController weaponController;
    KillableGroundNPC killableScript;
    public bool roamer;

    [Header("Movement and Pathfinding")]
    public float walkSpeed;
    public float runSpeed;
    public float slowWalkSpeed;
    public float nodeContactRadius;
    public Vector3 targetVelocity;
    private bool walking;
    public Vector3 currentFollowPoint;
    public Vector3 directionToPoint;
    Vector3 startPos;

    [Header("Visuals")]
    public float bobAmplitude;
    public float bobFrequency;
    public SpriteRenderer[] handSprites;
    public GameObject gunPivot;
    public PathfindingNode pathfindingNode;
    public Transform bodySpriteAnchor;

    [Header("Enemy Vision")]
    private GameObject searchPoint;
    public int fieldOfViewAngle;
    public float viewRaycastDistance;
    public LayerMask visionIgnoreLayers;
    public Vector3 lookVector;
    public Vector3 aimVector;
    private Vector3 aimVelocity = Vector3.zero;
    private Transform playerTransform;
    public float aimTime;

    [Header("Enemy Combat")]
    private float shootTimeElapsed;
    public float weaponRadius;
    public float stopRadius;
    public int burstBullets;
    public float timeBetweenBullets;
    public float timeBetweenBursts;
    private float currentBulletTimer;
    public float bulletRadius;
    private int shotsFired;

    
    //AI
    private State AIState;
    private float timeElapsed;
    private float wanderTimer;
    private float waitTimer;
    private float lookAngle;
    private bool seePlayer;
    private bool hasSeenPlayer;
    private bool wandering;

    public enum State 
    {
        Idle,
        Search,
        Attack
    }

    public void Die()
    {
        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.black;
        }
        gunPivot.SetActive(false);
        gameObject.layer = 14;
    }

    // Start is called before the first frame update
    void Start()
    {
        searchPoint = new GameObject("SearchPoint");
        targetVelocity = Vector3.zero;
        shotsFired = 0;
        currentBulletTimer = timeBetweenBullets;
        hasSeenPlayer = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        timeElapsed = 0f;
        shootTimeElapsed = 0f;
        wanderTimer = Random.Range(1f, 3f);
        waitTimer = Random.Range(1f, 3f);
        LookAngle(Random.Range(0f, 360f));
        aimVector = lookVector;
        wandering = roamer;
        killableScript = GetComponent<KillableGroundNPC>();
        walking = false;

        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.clear;
        }
        gunPivot.SetActive(true);
        currentFollowPoint = Vector2.zero;
        startPos = bodySpriteAnchor.localPosition;
    }

    private void Update()
    {
        if (killableScript.isDead) return;
        UpdateMovementBob();
        seePlayer = SeePlayer(viewRaycastDistance, fieldOfViewAngle, lookVector);
        if (seePlayer)
        {
            lookVector = (playerTransform.position - transform.position).normalized;
            if (HasShotOnPlayer(viewRaycastDistance, aimVector)) 
            {
                SwitchState(State.Attack);
            }
            else
            {
                SwitchState(State.Search);
                searchPoint.transform.position = playerTransform.position;
            }
            
        }
        else if (hasSeenPlayer)
        {
            SwitchState(State.Search);
        }
        else
        {
            SwitchState(State.Idle);
        }
        switch (AIState) 
        {
            case State.Idle:

                if (roamer) 
                {
                    if (wandering) 
                    {
                        if (timeElapsed <= wanderTimer) 
                        {
                            timeElapsed += Time.deltaTime;
                            targetVelocity = lookVector * walkSpeed;
                        }
                        else 
                        {
                            wanderTimer = Random.Range(1f, 3f);
                            timeElapsed = 0f;
                            wandering = false;
                            targetVelocity = Vector3.zero;
                            StartCoroutine(LookRandom(waitTimer/2));
                        }
                    }
                    else 
                    {
                        if (timeElapsed <= waitTimer) 
                        {
                            timeElapsed += Time.deltaTime;
                        }
                        else 
                        {
                            timeElapsed = 0f;
                            waitTimer = Random.Range(1f, 3f);
                            wandering = true;
                        }
                    }
                }
                else 
                {
                    if (timeElapsed <= waitTimer)
                    {
                        timeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        timeElapsed = 0f;
                        waitTimer = Random.Range(1f, 3f);
                        StartCoroutine(LookRandom(waitTimer / 2));
                    }
                }
                break;
            case State.Search:
                UpdatePathfind();
                break;
            case State.Attack:

                if (shootTimeElapsed <= currentBulletTimer)
                {
                    shootTimeElapsed += Time.deltaTime;
                }
                else
                {
                    weaponController.Fire();
                    shotsFired++;

                    if (shotsFired == burstBullets)
                    {
                        currentBulletTimer = timeBetweenBullets + timeBetweenBursts;
                        shotsFired = 0;
                    }
                    else
                    {
                        currentBulletTimer = timeBetweenBullets;
                    }
                    shootTimeElapsed = 0;
                }

                if (Vector3.Distance(playerTransform.position, transform.position) > weaponRadius)
                {
                    targetVelocity = lookVector * runSpeed;
                }
                else if (Vector3.Distance(playerTransform.position, transform.position) < stopRadius) 
                {
                    targetVelocity = Vector3.zero;
                }
                else
                {
                    targetVelocity = lookVector * slowWalkSpeed;
                }
                break;
        }
        SmoothAim();
        UpdateVelocity();
    }

    public void SwitchState(State nextState) 
    {
        if (nextState != AIState) 
        {
            switch (nextState) 
            {
                case State.Idle:
                    pathfindingNode.pathfindingEnabled = false;
                    hasSeenPlayer = false;
                    targetVelocity = Vector3.zero;
                    timeElapsed = 0f;
                    break;
                case State.Search:
                    pathfindingNode.pathfindingEnabled = true;
                    searchPoint.transform.position = playerTransform.position;
                    pathfindingNode.target = searchPoint.transform;
                    break;
                case State.Attack:
                    pathfindingNode.pathfindingEnabled = false;
                    hasSeenPlayer = true;
                    targetVelocity = Vector3.zero;
                    currentBulletTimer = 0f;
                    currentBulletTimer = timeBetweenBullets;
                    break;
            }
        }

        AIState = nextState;
    }
    private void UpdateVelocity() 
    {
        rb.velocity = targetVelocity;
    }

    IEnumerator LookRandom(float time)
    {
        yield return new WaitForSeconds(time);
        LookAngle(Random.Range(0, 360));
    }

    public void LookAngle(float angle)
    {
        lookAngle = angle;
        lookVector = (Quaternion.Euler(0, 0, angle) * transform.right);
    }


    public void SmoothAim() 
    {
        if (aimVector != lookVector) 
        {
            aimVector = Vector3.SmoothDamp(aimVector, lookVector, ref aimVelocity, aimTime);
        }
        weaponController.SetLookVector(aimVector);
    }

    public void UpdatePathfind() 
    {
        if (pathfindingNode.currentPath.Length > 0 && Vector3.Distance(transform.position, pathfindingNode.currentPath[pathfindingNode.currentPath.Length - 1]) == 0)
        {
            SwitchState(State.Idle);
            return;
        }
        else if (pathfindingNode.pointsIndex <= pathfindingNode.currentPath.Length - 1)
        {
            currentFollowPoint = pathfindingNode.currentPath[pathfindingNode.pointsIndex];
        }

        if (pathfindingNode.currentPath.Length > 0 && currentFollowPoint - transform.position != Vector3.zero) 
        {
            if (seePlayer) 
            {
                lookVector = lookVector = (playerTransform.position - transform.position).normalized;
            } 
            else 
            {
                lookVector = (currentFollowPoint - transform.position).normalized;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, currentFollowPoint, runSpeed * Time.deltaTime);
            directionToPoint = currentFollowPoint - transform.position;
        }

        if (Vector3.Distance(transform.position, currentFollowPoint) == 0)
        {
            pathfindingNode.pointsIndex++;
        }
    }

    public void UpdateMovementBob() 
    {
        walking = rb.velocity.magnitude > 0;
        if (!killableScript.isDead)
        {
            bodySpriteAnchor.localPosition += walking ? FootStepMotion(bobFrequency, bobAmplitude) : FootStepMotion(bobFrequency / 4, bobAmplitude / 6);
        }
        ResetPosition();
    }

    private bool SeePlayer(float distance, int angle, Vector3 direction)
    {

        for (int i = 0; i < angle; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (direction.normalized), distance, ~visionIgnoreLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (direction.normalized), distance, ~visionIgnoreLayers);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (direction.normalized) * 1);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (direction.normalized) * 1);
            if (upperHit && (upperHit.collider.CompareTag("Player"))) return true;
            if (lowerHit && (lowerHit.collider.CompareTag("Player"))) return true;
        }

        return false;
    }

    private bool HasShotOnPlayer(float distance, Vector3 direction) 
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, bulletRadius, (direction.normalized), distance, ~visionIgnoreLayers);
        Debug.DrawRay(transform.position, direction.normalized, Color.green);
        if (hit && (hit.collider.CompareTag("Player"))) return true;
        return false;
    }

    private Vector3 FootStepMotion(float bobFrequency, float bobAmplitude)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        return pos;
    }

    private void ResetPosition()
    {
        if (bodySpriteAnchor.localPosition == startPos) return;

        bodySpriteAnchor.localPosition = Vector3.Lerp(bodySpriteAnchor.localPosition, startPos, 1 * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SpaceStationWall") && wandering) 
        {
            lookVector = Vector3.Reflect(lookVector, collision.contacts[0].normal);
            lookAngle = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
        }
    }
}
