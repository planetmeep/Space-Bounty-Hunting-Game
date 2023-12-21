using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnseenCover : MonoBehaviour
{
    private Collider2D col;
    private List<GameObject> enemyList = new List<GameObject>();
    private List<GameObject> entityList = new List<GameObject>();
    private Transform playerTransform;
    private LayerMask castMask = new LayerMask();
    private int[] layers = {13, 15, 16};
    private bool exhausted = false;
    void Start()
    {
        foreach (int layer in layers) 
        {
            castMask |= (1 << layer);
        }
        col = GetComponent<Collider2D>();
        playerTransform = PlayerControlModes.instance.playerGround.transform;
    }


    private void ChangeEnemyVisibility(GameObject enemy, bool visible) 
    {
        GameObject bodySpriteGroup = enemy.transform.GetChild(0).gameObject;
        bodySpriteGroup = bodySpriteGroup.transform.GetChild(0).gameObject;
        SpriteRenderer[] spriteRenderers = bodySpriteGroup.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) 
        {
            spriteRenderer.sortingLayerName = visible ? "Foreground" : "Unseen";
        }

        GameObject gunSpriteGroup = enemy.transform.GetChild(1).gameObject;
        gunSpriteGroup = gunSpriteGroup.transform.GetChild(0).gameObject;
        spriteRenderers = gunSpriteGroup.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingLayerName = visible ? "Foreground" : "Unseen";
        }
    }

    /*private void Update()
    {
        if (!PlayerControlModes.instance.playerGround.activeSelf) return;
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, ((Vector3)col.ClosestPoint(playerTransform.position) - playerTransform.position).normalized, 
            Vector3.Distance(playerTransform.position, col.ClosestPoint(playerTransform.position - playerTransform.position)), castMask);

        if (hit && hit.collider.gameObject == gameObject) 
        {
            DestroyCover();
        }

    }*/

    public void DestroyCover() 
    {
        if (exhausted) return;
        exhausted = true;
        foreach (GameObject enemy in enemyList) 
        {
            ChangeEnemyVisibility(enemy, true);
        }
        //col.enabled = false;
        TriggerConnectors();
        Destroy(gameObject);
    }

    public void TriggerConnectors() 
    {
        List<UnseenConnector> nearestConnectors = new List<UnseenConnector>();
        GameObject[] connectors = GameObject.FindGameObjectsWithTag("UnseenConnector");
        foreach (GameObject connector in connectors)
        {
            if (connector.GetComponent<Collider2D>().bounds.Contains((Vector3)col.ClosestPoint(connector.transform.position)))
            {
                nearestConnectors.Add(connector.GetComponent<UnseenConnector>());
            }
        }

        foreach (UnseenConnector connector in nearestConnectors) 
        {
            connector.DestroySurroundingCover();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!enemyList.Contains(collision.gameObject))
            {
                enemyList.Add(collision.gameObject);
                ChangeEnemyVisibility(collision.gameObject, false);
            }
        }
    }

}
