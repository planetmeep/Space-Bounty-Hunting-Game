using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor; 
using UnityEngine;
using Pathfinding;


public class Bodyguard : MonoBehaviour, IKillable
{
    public Rigidbody2D rb;
    public AIPath AIPath;
    public AIDestinationSetter destinationSetter;
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
    public float bobSmoothTime;
    public SpriteRenderer[] handSprites;
    public GameObject gunPivot;
    public Transform bodySpriteAnchor;

    [Header("Enemy Vision")]
    private GameObject searchPoint;
    public int fieldOfViewAngle;
    public float viewRaycastDistance;
    public LayerMask visionIgnoreLayers;
    public Vector3 lookVector;
    public Vector3 aimVector;
    private Vector3 aimVelocity = Vector3.zero;
    private Vector3 bobVelocity = Vector3.zero;
    private Transform playerTransform;
    public float aimTime;

    [Header("Enemy Combat")]
    public float weaponRadius;
    private float shootTimeElapsed;
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
    private bool shotOnPlayer;
    private bool hasSeenPlayer;
    private bool wandering;

    public enum State
    {
        Idle,
        Search,
        Scan,
        Attack
    }

    public void Die()
    {
        AIPath.enabled = false;
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
        
        AIPath.enabled = false;
        hasSeenPlayer = false;
        shotOnPlayer = false;
        seePlayer = false;
        searchPoint = new GameObject("SearchPoint");
        destinationSetter.target = searchPoint.transform;
        targetVelocity = Vector3.zero;
        shotsFired = 0;
        currentBulletTimer = timeBetweenBullets;
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

        seePlayer = SeePlayer(viewRaycastDistance, fieldOfViewAngle, lookVector);
        shotOnPlayer = seePlayer ? HasShotOnPlayer(viewRaycastDistance, lookVector) : false;

        switch (AIState)
        {
            case State.Idle:

                if (seePlayer)
                {
                    if (shotOnPlayer)
                    {
                        SwitchState(State.Attack);
                    }
                    else
                    {
                        SwitchState(State.Search);
                    }
                }

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
                            StartCoroutine(LookRandom(waitTimer / 2));
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
                if (seePlayer)
                {
                    lookVector = (playerTransform.transform.position - transform.position).normalized;
                    if (shotOnPlayer)
                    {
                        SwitchState(State.Attack);
                    }
                    else
                    {

                        searchPoint.transform.position = playerTransform.position;
                        SwitchState(State.Search);
                    }
                } 
                else 
                {
                    lookVector = (AIPath.desiredVelocity).normalized;
                }

                if (AIPath.reachedEndOfPath)
                {
                    SwitchState(State.Scan);
                }

                break;
            case State.Scan:

                if (seePlayer) 
                {
                    StopAllCoroutines();
                    if (shotOnPlayer) 
                    {
                        SwitchState(State.Attack);
                    }
                    else 
                    {
                        searchPoint.transform.position = playerTransform.position;
                        SwitchState(State.Search);
                    }
                }

                if (timeElapsed <= 2f) 
                {
                    timeElapsed += Time.deltaTime;
                }
                else 
                {
                    StopAllCoroutines();
                    SwitchState(State.Idle);
                }
                break;
            case State.Attack:

                if (seePlayer)
                {
                    if (shotOnPlayer)
                    {
                        SwitchState(State.Attack);
                    }
                    else
                    {
                        SwitchState(State.Search);
                    }
                } 
                else 
                {
                    SwitchState(State.Search);
                }

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
                break;
        }
        UpdateMovementBob();
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
                    AIPath.enabled = false;
                    hasSeenPlayer = false;
                    targetVelocity = Vector3.zero;
                    timeElapsed = 0f;
                    break;
                case State.Search:
                    searchPoint.transform.position = playerTransform.position;
                    AIPath.enabled = true;
                    break;
                case State.Scan:
                    targetVelocity = Vector3.zero;
                    AIPath.enabled = false;
                    hasSeenPlayer = false;
                    timeElapsed = 0f;
                    StartCoroutine(ScanRandom(2f / 3f));
                    break;
                case State.Attack:
                    lookVector = (playerTransform.transform.position - transform.position).normalized;
                    AIPath.enabled = false;
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


    IEnumerator ScanRandom(float time)
    {
        yield return new WaitForSeconds(time);
        LookAngle(Random.Range(0, 360));
        yield return new WaitForSeconds(time);
        LookAngle(Random.Range(0, 360));
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

    public void UpdateMovementBob() 
    {
        walking = rb.velocity.magnitude > 0;
        if (!killableScript.isDead)
        {
            bodySpriteAnchor.localPosition += walking ? FootStepMotion(bobFrequency, bobAmplitude) : FootStepMotion(bobFrequency / 4, bobAmplitude / 2);
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
        RaycastHit2D circlehit = Physics2D.CircleCast(transform.position, bulletRadius, (direction.normalized), distance, ~visionIgnoreLayers);
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, (direction.normalized), distance, ~visionIgnoreLayers);
        Debug.DrawRay(transform.position, direction.normalized, Color.green);
        if (circlehit && (circlehit.collider.CompareTag("Player"))) 
        {
            if (rayhit && (rayhit.collider.CompareTag("Player"))) 
            {
                return true;
            }
        }
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

        bodySpriteAnchor.localPosition = Vector3.SmoothDamp(bodySpriteAnchor.localPosition, startPos, ref bobVelocity, bobSmoothTime);
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
