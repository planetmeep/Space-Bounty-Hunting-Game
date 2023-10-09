using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject owner;  // The game object that shot this projectile
    public float damageValue;

    // Made it set its owner so you can't shoot yourself
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         Debug.Log("Bullet collided with: " + collision.gameObject.name);
         IHittable hittable = collision.gameObject.GetComponent<IHittable>();
         if (hittable != null)
         {
             hittable.OnHit(this);
         }

         // Destroy the bullet upon collision
         Destroy(this.gameObject); 
    }
}