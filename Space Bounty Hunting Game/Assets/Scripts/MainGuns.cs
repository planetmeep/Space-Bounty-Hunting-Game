using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGuns : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // Drag your bullet prefab here in the inspector
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float rateOfFire = 5f; // Bullets per second
    [SerializeField] private float maxBulletDistance = 50f; // Max distance bullet can travel

    private float lastShootTime = 0; // To control the rate of fire

    public void Shoot()
    {
        if (Time.time - lastShootTime > 1/rateOfFire) // Check if enough time has passed since the last shot
        {
            lastShootTime = Time.time;

            // Create the bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            // Set its velocity
            rb.velocity = transform.up * bulletSpeed; // Assuming the forward direction of your ship is its "up" in 2D space

            // Automatically destroy bullet after traveling its max distance
            Destroy(bullet, maxBulletDistance / bulletSpeed);
        }
    }
}