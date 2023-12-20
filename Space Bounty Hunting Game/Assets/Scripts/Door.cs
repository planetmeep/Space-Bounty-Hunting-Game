using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Collider2D[] colliders;
    public GameObject coverTileGroup;
    public Animator animator;
    private bool playerContact;
    public bool vertical;
    private LayerMask castMask = new LayerMask();
    private int[] layers = { 13, 16 };

    void Start()
    {
        foreach (int layer in layers)
        {
            castMask |= (1 << layer);
        }
        playerContact = false;
    }

    void Update()
    {
        if (playerContact) 
        {
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                AudioManager.instance.PlaySound("DoorOpen");
                ScreenShake.Instance.ShakeCamera(0.3f, 0.2f);
                animator.Play("Open");
                coverTileGroup.SetActive(false);
                DestroySurroundingCover();
                foreach (Collider2D collider in colliders) 
                {
                    collider.enabled = false;
                }      
            }
        }
    }

    public void DestroySurroundingCover()
    {
        if (vertical)
        {
            RaycastHit2D rightHit = Physics2D.Raycast(transform.parent.position, transform.parent.right, 1f, castMask);
            RaycastHit2D leftHit = Physics2D.Raycast(transform.parent.position, -transform.parent.right, 1f, castMask);
            DestroyHitCover(rightHit, leftHit);
        }
        else
        {
            RaycastHit2D upHit = Physics2D.Raycast(transform.parent.position, transform.parent.up, 1f, castMask);
            RaycastHit2D downHit = Physics2D.Raycast(transform.parent.position, -transform.parent.up, 1f, castMask);
            DestroyHitCover(upHit, downHit);
        }

    }

    private void DestroyHitCover(RaycastHit2D hit1, RaycastHit2D hit2)
    {
        if (hit1 && hit1.collider.CompareTag("UnseenCover"))
        {
            hit1.transform.gameObject.GetComponent<UnseenCover>().DestroyCover();
        }

        if (hit2 && hit2.collider.CompareTag("UnseenCover"))
        {
            hit2.transform.gameObject.GetComponent<UnseenCover>().DestroyCover();
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
