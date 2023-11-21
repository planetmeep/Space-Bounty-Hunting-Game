using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IHittable
{
    public float health = 100f;
    public ShipGuns mainGuns;
    public float detectionRadius = 10f;  // The distance at which the enemy recognizes the player
    public float WeaponsRadius = 10f;
    public float stopFollowRadius = 10f;
    public Transform playerTransform;   // Reference to the player's transform
    public GameObject explosionPrefab;
    public HitsoundMaterials hitsoundMaterial;
    public Engines engines;
    public Rigidbody2D rb;
    public virtual void UpdateBehavior() { }


    protected void Start()
    {
        // Assuming there's only one player and it's tagged as "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    protected void Update()
    {
        UpdateBehavior();
    }

    public void OnHit(Projectile projectile, Vector2 hitPoint) 
    {
        AudioManager.instance.PlayImpactSound(hitsoundMaterial);
        Debug.Log("HIT");
        health -= projectile.damageValue;

        if (health <= 0)
        {
            // Handle the death of the enemy
            Instantiate(explosionPrefab, hitPoint, Quaternion.identity);
            AudioManager.instance.ResetPlaySound("Explosion");
            Destroy(this.gameObject);
        }
    }

    protected void CheckAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            AttackPlayer(playerTransform);
        }
    }
    protected bool CheckPlayerWithinWeaponsRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= WeaponsRadius)
        {
            return true;
        }
        return false;
    }

    protected bool CheckPlayerWithinDetectionRange() 
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            return true;
        }
        return false;
    }

    protected bool CheckPlayerOutsideStopFollow() 
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer >= stopFollowRadius)
        {
            return true;
        }
        return false;
    }

    protected void AttackPlayer(Transform playerTransform)
    {
        if(mainGuns == null)
        {
            Debug.LogError("MainGuns not assigned!");
            return;
        }
        mainGuns.ShootAt(playerTransform.position);
    }
    protected void MoveTowardsPlayer()
    {
        if(engines == null)
        {
            Debug.Log("NO ENGINES CANT FLY");
            return;
        }
        // Calculate the direction to the player.
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Thrust in that direction.
        engines.Thrust(directionToPlayer);
    }
    protected void FacePlayer()
    {
    // Calculate direction from enemy to player
    Vector2 directionToPlayer = playerTransform.position - transform.position;

    // Calculate the rotation in radians
    float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

    // Apply the rotation to the enemy's transform
    rb.MoveRotation(angle - 90); // Subtract 90 degrees if the sprite is facing upwards by default.
    }

    public void OnHit(Projectile projectile, Vector2 hitPoint, Quaternion hitDirection)
    {
        throw new System.NotImplementedException();
    }
}