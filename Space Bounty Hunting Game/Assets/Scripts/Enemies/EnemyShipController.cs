using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipController : MonoBehaviour
{
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float brakeFactor = 0.9f;  // Multiplier to reduce speed. Value between 0 (full stop) and 1 (no braking).
    [SerializeField] private ParticleSystem thrusterParticles;
    private Rigidbody2D rb;
    private float currentVelocity;
    private float timeAccelerated;
    public bool thrustForward = false;
    public bool applyBrakes = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentVelocity = 0;
        timeAccelerated = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
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

            /*if (!AudioManager.instance.GetAudioSource("Thruster").isPlaying)
            {
                AudioManager.instance.PlaySound("Thruster");
            }*/
            ThrustForward();
        }
        else
        {
            thrusterParticles.Stop();
            timeAccelerated = 0;
            //AudioManager.instance.StopSound("Thruster");
        }


        if (applyBrakes)
        {
            ApplyBrakes();
        }
    }

    private void ThrustForward()
    {
        Vector2 thrustDirection = transform.up;
        rb.velocity = thrustDirection * currentVelocity;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
    }

    private void ApplyBrakes()
    {
        rb.velocity *= brakeFactor;
    }

}
