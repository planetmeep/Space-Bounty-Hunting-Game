using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Collider2D[] colliders;
    public Animator animator;
    private bool playerContact;

    void Start()
    {
        playerContact = false;
    }

    void Update()
    {
        if (playerContact) 
        {
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                ScreenShake.Instance.ShakeCamera(0.3f, 0.2f);
                animator.Play("Open");
                foreach (Collider2D collider in colliders) 
                {
                    collider.enabled = false;
                }      
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            playerContact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerContact = false;
        }
    }
}
