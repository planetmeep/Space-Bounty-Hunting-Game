using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGuns : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // Drag your bullet prefab here in the inspector
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float rateOfFire = 5f; // Bullets per second
    [SerializeField] private float maxBulletDistance = 50f; // Max distance bullet can travel
    [SerializeField] private Collider2D shipCollider;//SET IN INSPECTOR

    private float lastShootTime = 0; // To control the rate of fire

    Vector2 GetSafeSpawnPosition(Vector2 direction)
    {
        float safeDistance = shipCollider.bounds.extents.magnitude + 0.1f;  // collider's half size + some extra offset
        return (Vector2)transform.position + (direction.normalized * safeDistance);
    }
    public void Shoot()
    {
        if (Time.time - lastShootTime > 1/rateOfFire) // Check if enough time has passed since the last shot
        {
            FireBullet(transform.up * bulletSpeed); // Assuming the forward direction of your ship is its "up" in 2D space
        }
    }

    public void ShootAt(Vector2 targetPosition)
    {
        if (Time.time - lastShootTime > 1/rateOfFire) // Check if enough time has passed since the last shot
        {
            // Calculate direction from gun to target
            Vector2 shootDirection = (targetPosition - (Vector2)transform.position).normalized;

            FireBullet(shootDirection * bulletSpeed); 
        }
    }

    private void FireBullet(Vector2 velocity)
    {
        lastShootTime = Time.time;

        // Create the bullet at a safe position
        Vector2 spawnPosition = GetSafeSpawnPosition(velocity);
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Get the projectile and rigidbody components only once
        Projectile projectile = bullet.GetComponent<Projectile>();
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Set bullet velocity
        rb.velocity = velocity;

        // Automatically destroy bullet after traveling its max distance
        Destroy(bullet, maxBulletDistance / bulletSpeed);
    }
}