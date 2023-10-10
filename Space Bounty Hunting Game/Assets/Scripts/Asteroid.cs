using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IHittable
{
    const float LARGE_ASTEROID_HP = 100f;
    const float MEDIUM_ASTEROID_HP = 50f;
    const float SMALL_ASTEROID_HP = 20f;
    const float TINY_ASTEROID_HP = 10f;

    const float LARGE_EXPLOSION_RADIUS = 100f;
    const float MEDIUM_EXPLOSION_RADIUS = 50f;
    const float SMALL_EXPLOSION_RADIUS = 20f;
    const float ASTEROID_EXPLOSION_FORCE = 100f;

    public GameObject[] asteroidPrefabs;
    public Transform[] explosionAsteroidSpawns;

    public HitsoundMaterials hitsoundMaterial;
    public AsteroidSizes asteroidSize;

    private float asteroidHP;
    private float explosionRadius;

    private void Start()
    {
        switch ((int)asteroidSize) 
        {
            case 0:
                asteroidHP = TINY_ASTEROID_HP; 
                break;
            case 1:
                asteroidHP = SMALL_ASTEROID_HP;
                explosionRadius = SMALL_EXPLOSION_RADIUS; 
                break;
            case 2:
                asteroidHP = MEDIUM_ASTEROID_HP;
                explosionRadius = MEDIUM_EXPLOSION_RADIUS;
                break;
            case 3:
                asteroidHP = LARGE_ASTEROID_HP;
                explosionRadius = LARGE_EXPLOSION_RADIUS;
                break;
        }
    }
    public enum AsteroidSizes 
    {
        Large = 3,
        Medium = 2,
        Small = 1,
        Tiny = 0
    }

    public void OnHit(Projectile projectile)
    {
        AudioManager.instance.PlayImpactSound(hitsoundMaterial);
        asteroidHP -= projectile.damageValue;
        
        if (asteroidHP <= 0) 
        {
            AsteroidDie();
        }
    }

    private void AsteroidDie() 
    {
        Debug.Log((int)asteroidSize);
        AudioManager.instance.ResetPlaySound("Explosion");
        if ((int)asteroidSize > 0) 
        {
            foreach(Transform spawnPoint in explosionAsteroidSpawns) 
            {
                float randZRotation = Random.Range(0, 360f);
                var currentAsteroid = Instantiate(asteroidPrefabs[(int)asteroidSize - 1], spawnPoint.position, Quaternion.Euler(0, 0, randZRotation));
                Rigidbody2D asteroidRigidbody = currentAsteroid.GetComponent<Rigidbody2D>();
                Rigidbody2DExtensions.AddExplosionForce2D(asteroidRigidbody, transform.position, ASTEROID_EXPLOSION_FORCE, explosionRadius);
            }
        }
        Destroy(gameObject);
    }
}
