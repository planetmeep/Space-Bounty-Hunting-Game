using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGuns : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // Drag your bullet prefab here in the inspector
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float rateOfFire = 5f; // Bullets per second
    [SerializeField] private float fireRateRandomness = 1f;
    [SerializeField] private float maxBulletDistance = 50f; // Max distance bullet can travel
    [SerializeField] private Collider2D shipCollider;//SET IN INSPECTOR
    [SerializeField] private int projectileLayer;
    [SerializeField] private bool randomROF;

    private float currentROF;
    private float lastShootTime = 0; // To control the rate of fire

    private void Start()
    {
        if (randomROF) currentROF = Random.Range(rateOfFire - fireRateRandomness, rateOfFire + fireRateRandomness);
        else currentROF = rateOfFire;
    }
    Vector2 GetSafeSpawnPosition(Vector2 direction)
    {
        float safeDistance = shipCollider.bounds.extents.magnitude + 0.1f;  // collider's half size + some extra offset
        return (Vector2)transform.position + (direction.normalized * safeDistance);
    }
    public void Shoot()
    {
        if (Time.time - lastShootTime > 1/currentROF) // Check if enough time has passed since the last shot
        {
            AudioManager.instance.PlaySound("PlayerShipShot");
            FireBullet(transform.up, transform.rotation);
        }
    }

    public void ShootAt(Vector2 targetPosition)
    {
        if (Time.time - lastShootTime > 1/currentROF) // Check if enough time has passed since the last shot
        {
            // Calculate direction from gun to target
            Vector2 shootDirection = (targetPosition - (Vector2)transform.position).normalized;
            Quaternion bulletRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, shootDirection) - 90);
            FireBullet(shootDirection, bulletRotation); 
        }
    }

    private void FireBullet(Vector2 direction, Quaternion bulletRotation)
    {
        lastShootTime = Time.time;

        // Create the bullet at a safe position
        Vector2 spawnPosition = GetSafeSpawnPosition(direction);
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletRotation);

        // Get the projectile and rigidbody components only once
        Projectile projectile = bullet.GetComponent<Projectile>();
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;
        projectile.SetOwner(gameObject, projectileLayer);

        // Automatically destroy bullet after traveling its max distance
        Destroy(bullet, maxBulletDistance / bulletSpeed);
    }
}