using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float thrustPower = 35f;
    [SerializeField] private float rotationSpeed = 220f;
    [SerializeField] private float maxVelocity = 10f;

    private Rigidbody2D rb;

    private bool rotateLeft = false;
    private bool rotateRight = false;
    private bool thrustForward = false;
    private bool thrustBackward = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

     private void FixedUpdate()
    {
        if (rotateLeft)
        {
            RotateLeft();
        }
        if (rotateRight)
        {
            RotateRight();
        }
        if (thrustForward)
        {
            ThrustForward();
        }
        if (thrustBackward)
        {
            ThrustBackward();
        }
    }

    
    public void StartThrustForward()
    {
        thrustForward = true;
    }

    public void StopThrustForward()
    {
        thrustForward = false;
    }

    public void StartThrustBackward()
    {
        thrustBackward = true;
    }

    public void StopThrustBackward()
    {
        thrustBackward = false;
    }


    public void StartRotateLeft()
    {
        rotateLeft = true;
    }

    public void StopRotateLeft()
    {
        rotateLeft = false;
    }

    public void StartRotateRight()
    {
        rotateRight = true;
    }

    public void StopRotateRight()
    {
        rotateRight = false;
    }

    

    private void RotateLeft()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
    }

    private void RotateRight()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime);
    }
    private void ThrustForward()
    {
        Vector2 thrustDirection = transform.up;
        rb.AddForce(thrustDirection * thrustPower, ForceMode2D.Force);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }

    private void ThrustBackward()
    {
        Vector2 thrustDirection = -transform.up;
        rb.AddForce(thrustDirection * thrustPower, ForceMode2D.Force);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }
}