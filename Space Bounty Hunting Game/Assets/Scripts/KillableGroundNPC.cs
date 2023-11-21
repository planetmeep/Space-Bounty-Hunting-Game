using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableGroundNPC : MonoBehaviour, IHittable
{
    public Animator animator;
    public float health;
    public float deathPushForce;
    public HitsoundMaterials hitsoundMaterial;
    public GameObject hitParticle;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnHit(Projectile projectile, Vector2 hitPoint, Quaternion hitDirection)
    {
        AudioManager.instance.PlayImpactSound(hitsoundMaterial);
        health -= projectile.damageValue;

        if (health <= 0)
        {
            // Handle the death of the enemy
            animator.Play("Death");
            Instantiate(hitParticle, hitPoint, hitDirection);
            Vector2 projectileToNPC = transform.position - (Vector3)hitPoint;
            rb.AddForce(projectileToNPC * deathPushForce);
        }
    }
}
