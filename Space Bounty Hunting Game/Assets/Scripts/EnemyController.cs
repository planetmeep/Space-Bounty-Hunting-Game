using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHittable
{
    public float health = 100f;
    public MainGuns mainGuns;
    public float detectionRadius = 10f;  // The distance at which the enemy recognizes the player
    private Transform playerTransform;   // Reference to the player's transform
    public HitsoundMaterials hitsoundMaterial;

    private void Start()
    {
        // Assuming there's only one player and it's tagged as "Player"
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerTransform == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    private void Update()
    {
        CheckAndAttackPlayer();
    }

    public void OnHit(Projectile projectile) 
    {
        AudioManager.instance.PlayImpactSound(hitsoundMaterial);
        Debug.Log("HIT");
        health -= projectile.damageValue;

        if (health <= 0)
        {
            // Handle the death of the enemy
            Destroy(this.gameObject);
        }
    }

    private void CheckAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            AttackPlayer(playerTransform);
        }
    }

    private void AttackPlayer(Transform playerTransform)
    {
        if(mainGuns == null)
        {
            Debug.LogError("MainGuns not assigned!");
            return;
        }
        mainGuns.ShootAt(playerTransform.position);
    }
}
