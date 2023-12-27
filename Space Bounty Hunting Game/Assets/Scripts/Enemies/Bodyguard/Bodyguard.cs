using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float bobAmplitudeDivisor;
    [SerializeField] private float bobFrequencyDivisor;
    [SerializeField] private float bobLerpTime;
    private float currentAmplitude;
    private float currentFrequency;
    private float lerpTimer;
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
    private Vector3 desiredBobPosition = Vector3.zero;
    private Transform playerTransform;
    public float aimTime;

    [Header("Enemy Combat")]
    public float weaponRadius;
    private float shootTimeElapsed;
    public float stopRadius;
    public int burstBullets;
    public float timeBetweenBullets;
    public float timeBetweenBursts;
    public float reactionTime;
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
        lookVector = transform.up;
        lerpTimer = 0f;
        AIPath.enabled = false;
        hasSeenPlayer = false;
        shotOnPlayer = false;
        seePlayer = false;
        searchPoint = new GameObject("SearchPoint");
        destinationSetter.target = searchPoint.transform;
        targetVelocity = Vector3.zero;
        shotsFired = 0;
        currentBulletTimer = timeBetweenBullets;
        playerTransform = PlayerControlModes.instance.playerGround.transform;
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
        if (killableScript.isDead || !PlayerControlModes.instance.playerGround.activeSelf) return;

        seePlayer = SeePlayer(viewRaycastDistance, fieldOfViewAngle, lookVector);
        shotOnPlayer = seePlayer ? HasShotOnPlayer(viewRaycastDistance, lookVector): false;
        lookVector = (seePlayer || shotOnPlayer) ? (playerTransform.transform.position - transform.position).normalized : lookVector; 

        bool newWalking = false;
        bool running = false;

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
                        newWalking = true;
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
                        newWalking = false;
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
                    newWalking = false;
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
                newWalking = true;
                running = true;
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
                newWalking = false;

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

                if (timeElapsed <= 0.5f) 
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
                targetVelocity = Vector3.zero;
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
        UpdateMovementBob(newWalking, running);
        SmoothAim();
        UpdateVelocity();
        walking = newWalking;
    }

    public void SwitchState(State nextState)
    {
        if (nextState != AIState)
        {
            targetVelocity = Vector3.zero;
            switch (nextState)
            {
                case State.Idle:
                    AIPath.enabled = false;
                    hasSeenPlayer = false;
                    timeElapsed = 0f;
                    break;
                case State.Search:
                    searchPoint.transform.position = playerTransform.position;
                    AIPath.enabled = true;
                    break;
                case State.Scan:
                    AIPath.enabled = false;
                    hasSeenPlayer = false;
                    timeElapsed = 0f;
                    StartCoroutine(LookRandom(0.25f));
                    break;
                case State.Attack:
                    AIPath.enabled = false;
                    hasSeenPlayer = true;
                    currentBulletTimer = 0f;
                    currentBulletTimer = reactionTime;
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

    public void UpdateMovementBob(bool newWalking, bool running) 
    {
        bool walkingChanged = CheckChangeWalking(newWalking);

        lerpTimer += Time.deltaTime;
        lerpTimer = Mathf.Min(lerpTimer, bobLerpTime);

        if (walkingChanged) lerpTimer = 0f;

        if (walking && running) 
        {
            currentAmplitude = bobAmplitude * 1.5f;
            currentFrequency = bobFrequency * 2;
        }
        else if (walking)
        {
            currentAmplitude = bobAmplitude;
            currentFrequency = bobFrequency;
        }
        else
        {
            currentAmplitude = bobAmplitude / bobAmplitudeDivisor;
            currentFrequency = bobFrequency / bobFrequencyDivisor;
        }

        desiredBobPosition = FootStepMotion(currentFrequency, currentAmplitude);

        bodySpriteAnchor.localPosition = Vector2.Lerp(bodySpriteAnchor.localPosition, desiredBobPosition, lerpTimer / bobLerpTime);
    }

    private bool CheckChangeWalking(bool newWalking)
    {
        if (newWalking != walking)
        {
            return true;
        }
        return false;
    }

    private bool SeePlayer(float distance, int angle, Vector3 direction)
    {

        for (int i = 0; i < angle; i++)
        {
            RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * (direction.normalized), distance, ~visionIgnoreLayers);
            RaycastHit2D lowerHit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -i) * (direction.normalized), distance, ~visionIgnoreLayers);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * (direction.normalized) * viewRaycastDistance);
            Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -i) * (direction.normalized) * viewRaycastDistance);
            if (upperHit && (upperHit.collider.CompareTag("Player"))) return true;
            if (lowerHit && (lowerHit.collider.CompareTag("Player"))) return true;
        }

        return false;
    }

    private bool HasShotOnPlayer(float distance, Vector3 direction) 
    {
        RaycastHit2D circlehit = Physics2D.CircleCast(transform.position, bulletRadius, (playerTransform.position - transform.position).normalized, distance, ~visionIgnoreLayers);
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, distance, ~visionIgnoreLayers);

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

    private Vector3 FootStepMotion(float bobFrequency, float bobAmplitude)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        return pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.CompareTag("SpaceStationWall") || collision.collider.CompareTag("Door")) && wandering) 
        {
            lookVector = Vector3.Reflect(lookVector, collision.contacts[0].normal);
            lookAngle = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
        }
    }
}
