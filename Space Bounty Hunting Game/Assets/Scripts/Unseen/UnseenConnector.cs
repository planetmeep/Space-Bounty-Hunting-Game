using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnseenConnector : MonoBehaviour
{
    public bool vertical;
    private LayerMask castMask = new LayerMask();
    private int[] layers = { 13, 15, 16 };
    private bool exhausted = false;

    void Start()
    {
        foreach (int layer in layers)
        {
            castMask |= (1 << layer);
        }
    }

    public void DestroySurroundingCover() 
    {
        if (exhausted) return;
        exhausted = true;
        if (vertical) 
        {
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, transform.right, 1f, castMask);
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, -transform.right, 1f, castMask);
            DestroyHitCover(rightHit, leftHit);
        } 
        else 
        {
            RaycastHit2D upHit = Physics2D.Raycast(transform.position, transform.up, 1f, castMask);
            RaycastHit2D downHit = Physics2D.Raycast(transform.position, -transform.up, 1f, castMask);
            DestroyHitCover(upHit, downHit);
        }
        Destroy(gameObject);

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
}
