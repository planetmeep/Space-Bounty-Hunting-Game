using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGuns : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject muzzleParticles;
    public ParticleSystem muzzleFlash;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float rateOfFire = 5f;
    [SerializeField] private int projectileLayer;
    [SerializeField] private float maxBulletDistance = 50f;
    private float lastShootTime = 0;

    // Update is called once per frame
    public void Shoot()
    {
        AudioManager.instance.PlaySound("Pistol");
        Vector3 pointDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float mouseAngle = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
        FireBullet(pointDirection, Quaternion.Euler(0, 0, mouseAngle - 90));
        muzzleFlash.Play();
        ScreenShake.Instance.ShakeCamera(0.175f, 0.125f);
    }
    private void FireBullet(Vector2 direction, Quaternion bulletRotation)
    {
        lastShootTime = Time.time;

        // Create the bullet at a safe position
        //Vector2 spawnPosition = GetSafeSpawnPosition(direction);
        GameObject bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);

        // Get the projectile and rigidbody components only once
        Projectile projectile = bullet.GetComponent<Projectile>();
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * bulletSpeed;
        projectile.SetOwner(gameObject, projectileLayer);

        // Automatically destroy bullet after traveling its max distance
        Destroy(bullet, maxBulletDistance / bulletSpeed);
    }
}
