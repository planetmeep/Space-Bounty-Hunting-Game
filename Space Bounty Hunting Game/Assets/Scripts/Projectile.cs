using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Hit particle system prefab
    public GameObject hitParticle;
    public GameObject owner;  // The game object that shot this projectile
    public float damageValue;

    // Made it set its owner so you can't shoot yourself
    public void SetOwner(GameObject newOwner, int projectileLayer)
    {
        owner = newOwner;
        //gameObject.layer = newOwner.layer;
        gameObject.layer = projectileLayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         ContactPoint2D contactPoint = collision.GetContact(0);
         Vector2 contactPosition = contactPoint.point; 
         Debug.Log("Bullet collided with: " + collision.gameObject.name);
         Instantiate(hitParticle, contactPosition, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180));

        IHittable hittable = collision.gameObject.GetComponent<IHittable>();
         if (hittable != null)
         {
             hittable.OnHit(this, contactPosition);
         }

         // Destroy the bullet upon collision
         Destroy(this.gameObject); 
    }
}