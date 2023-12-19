using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bodySpriteGroup;
    [SerializeField] private float bobAmplitude;
    [SerializeField] private float bobFrequency;
    [SerializeField] private float bobAmplitudeDivisor;
    [SerializeField] private float bobFrequencyDivisor;
    [SerializeField] private float bobLerpTime;
    private float currentAmplitude;
    private float currentFrequency;
    private float lerpTimer;
    private Rigidbody2D rb;
    private float timeMoved;
    Vector2 movementVelocity = Vector2.zero;
    Vector2 desiredPosition = Vector2.zero;
    Vector3 startPos;
    private bool walking;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walking = false;
        startPos = bodySpriteGroup.localPosition;
        lerpTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        movementVelocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
        bool walkingChanged = CheckChangeWalking(movementVelocity.magnitude > 0);
        walking = movementVelocity.magnitude > 0;


        lerpTimer += Time.deltaTime;
        lerpTimer = Mathf.Min(lerpTimer, bobLerpTime);

        if (walkingChanged) lerpTimer = 0f;

        if (walking) 
        {
            currentAmplitude = bobAmplitude;
            currentFrequency = bobFrequency;
        } 
        else 
        {
            currentAmplitude = bobAmplitude / bobAmplitudeDivisor;
            currentFrequency = bobFrequency / bobFrequencyDivisor;
        }

        desiredPosition = FootStepMotion(currentFrequency, currentAmplitude);

        bodySpriteGroup.localPosition = Vector2.Lerp(bodySpriteGroup.localPosition, desiredPosition, lerpTimer / bobLerpTime);


    }
    private Vector3 FootStepMotion(float bobFrequency, float bobAmplitude)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        return pos;
    }

    private bool CheckChangeWalking(bool newWalking) 
    {
        if (newWalking != walking) 
        {
            return true;
        }
        return false;
    }
    private void FixedUpdate()
    {
        rb.velocity = movementVelocity;
    }
}
