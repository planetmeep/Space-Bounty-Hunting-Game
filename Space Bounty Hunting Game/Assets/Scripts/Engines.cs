using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Engines : MonoBehaviour
{
    [SerializeField] private ParticleSystem thrusterParticles;
    [SerializeField] private float velocityRandomness = 0.2f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float thrustPower = 35f;
    [SerializeField] private float brakeFactor = 0.9f;  // Multiplier to reduce speed. Value between 0 (full stop) and 1 (no braking).
    private float currentThrustPower;
    private Rigidbody2D rb;

    private void Start()
    {
        currentThrustPower = Random.Range(thrustPower - velocityRandomness, thrustPower + velocityRandomness);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Thrust(Vector2 thrustDirection)
    {
        rb.AddForce(thrustDirection * currentThrustPower, ForceMode2D.Force);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }
    public void ApplyBrakes()
    {
        rb.velocity *= brakeFactor;
    }

}