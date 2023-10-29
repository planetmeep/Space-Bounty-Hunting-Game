using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float brakeFactor = 0.9f;  // Multiplier to reduce speed. Value between 0 (full stop) and 1 (no braking).
    [SerializeField] public MainGuns mainGuns;
    private bool isShooting = false;
    [SerializeField] private float lateralThrustPower = 3f;
    [SerializeField] private ParticleSystem thrusterParticles;
    public Transform[] directionalPoints; 
    private Rigidbody2D rb;
    private float currentVelocity;
    private float timeAccelerated;
    private bool thrustForward = false;
    private bool applyBrakes = false;
    private bool thrustLeft = false;
    private bool thrustRight = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentVelocity = 0;
        timeAccelerated = 0;
    }

    private void FixedUpdate()
    {
        if (thrustForward)
        {
            thrusterParticles.Play();

            //currentVelocity = Mathf.Lerp(currentVelocity, maxVelocity, timeAccelerated / accelerationTime);
            if (timeAccelerated <= accelerationTime) 
            {
                currentVelocity = Mathf.Lerp(rb.velocity.magnitude, maxVelocity, timeAccelerated / accelerationTime);
                timeAccelerated += Time.deltaTime;
            }
            
            if (!AudioManager.instance.GetAudioSource("Thruster").isPlaying) 
            {
                AudioManager.instance.PlaySound("Thruster");
            }
            ThrustForward();
        }
        else 
        {
            thrusterParticles.Stop();
            timeAccelerated = 0;
            AudioManager.instance.StopSound("Thruster");
        }

        if (applyBrakes)
        {
            ApplyBrakes();
        }
         if (thrustLeft)
        {
            ApplyLeftThrust();
        }

        if (thrustRight)
        {
            ApplyRightThrust();
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
        //rb.AddForce(thrustDirection * thrustPower, ForceMode2D.Force);
        rb.velocity = thrustDirection * currentVelocity;
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
        //Debug.Log("PEW PEW");
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
        rb.MoveRotation(angle);
    }
public void StartLeftThrust()
{
    thrustLeft = true;
}

public void StopLeftThrust()
{
    thrustLeft = false;
}

public void StartRightThrust()
{
    thrustRight = true;
}

public void StopRightThrust()
{
    thrustRight = false;
}

private void ApplyLeftThrust()
{
    Vector2 thrustDirection = -transform.right; // Negative right direction is left
    rb.AddForce(thrustDirection * lateralThrustPower, ForceMode2D.Force);
    rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
}

private void ApplyRightThrust()
{
    Vector2 thrustDirection = transform.right;
    rb.AddForce(thrustDirection * lateralThrustPower, ForceMode2D.Force);
    rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
}
}