using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodyguard : MonoBehaviour, IKillable
{
    public SpriteRenderer[] handSprites;
    public GameObject gunPivot;
    public PathfindingNode pathfindingNode;
    public Rigidbody2D rb;
    public Vector3 currentFollowPoint;
    public Vector3 directionToPoint;
    public float movementSpeed;
    public float nodeContactRadius;
    public void Die()
    {
        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.black;
        }
        gunPivot.SetActive(false);
        movementSpeed = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.clear;
        }
        gunPivot.SetActive(true);
        currentFollowPoint = Vector2.zero;
    }

    private void Update()
    {
        currentFollowPoint = pathfindingNode.currentPath[pathfindingNode.pointsIndex];
        directionToPoint = currentFollowPoint - transform.position;

        //transform.position = Vector3.MoveTowards(transform.position, currentFollowPoint, movementSpeed * Time.deltaTime);

        //rb.velocity = directionToPoint.normalized * movementSpeed;

        if (Vector3.Distance(transform.position, currentFollowPoint) <= nodeContactRadius) 
        {
            pathfindingNode.pointsIndex++;
        }
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            currentFollowPoint = pathfindingNode.currentPath[pathfindingNode.pointsIndex];
            pathfindingNode.pointsIndex++;
            transform.position = currentFollowPoint;
        }
    }


    private void FixedUpdate()
    {
        rb.velocity = directionToPoint.normalized * movementSpeed;
    }
}
