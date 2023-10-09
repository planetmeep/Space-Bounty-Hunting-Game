using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float thrustPower = 35f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float brakeFactor = 0.9f;  // Multiplier to reduce speed. Value between 0 (full stop) and 1 (no braking).
    [SerializeField] public MainGuns mainGuns;
    private bool isShooting = false;

    private Rigidbody2D rb;

    private bool thrustForward = false;
    private bool applyBrakes = false;

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
        if (applyBrakes)
        {
            ApplyBrakes();
        }
    }
    private void Update()
    {
        if (isShooting)
        {
        MainGuns();
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

    public void StartBraking()
    {
        applyBrakes = true;
    }

    public void StopBraking()
    {
        applyBrakes = false;
    }

    private void ThrustForward()
    {
        Vector2 thrustDirection = transform.up;
        rb.AddForce(thrustDirection * thrustPower, ForceMode2D.Force);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }
    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }
    public void MainGuns()
    {
        Debug.Log("PEW PEW");
        mainGuns.Shoot();
    }

    private void ApplyBrakes()
    {
        rb.velocity *= brakeFactor;
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