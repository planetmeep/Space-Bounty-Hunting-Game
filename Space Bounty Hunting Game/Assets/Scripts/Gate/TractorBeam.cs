using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{

    public bool playerColliding;

    private void Start()
    {
        playerColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerShip")) 
        {
            playerColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerShip"))
        {
            playerColliding = false;
        }
    }
}
