using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float thrustPower = 35f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxVelocity = 10f;

    private Rigidbody2D rb;

    private bool thrustForward = false;
    private bool thrustBackward = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

     private void FixedUpdate()
    {
        
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
    public void UpdateMousePosition(Vector2 mousePosition)
    {
        // Convert mouse position to world position
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        
        // Calculate the direction from ship to mouse
        Vector2 direction = (worldMousePos - transform.position).normalized;
        
        // Calculate the rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Subtracting 90 degrees to adjust the rotation
        
        // Set rotation
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }
}