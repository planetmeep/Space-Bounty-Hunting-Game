using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Engines : MonoBehaviour
{
    [SerializeField] private float lateralThrustPower = 3f;
    [SerializeField] private ParticleSystem thrusterParticles;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float thrustPower = 35f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Thrust(Vector2 thrustDirection)
    {
    
        // Calculate thrust power based on direction
        float currentThrustPower = thrustDirection.x != 0 ? lateralThrustPower : thrustPower;
        rb.AddForce(thrustDirection * currentThrustPower, ForceMode2D.Force);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }
}