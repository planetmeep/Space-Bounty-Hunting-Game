using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bodyguard : MonoBehaviour, IKillable
{
    public SpriteRenderer[] handSprites;
    public GameObject gunPivot;
    public PathfindingNode pathfindingNode;
    public Transform bodySpriteAnchor;
    public Rigidbody2D rb;
    public Vector3 currentFollowPoint;
    public Vector3 directionToPoint;
    Vector3 startPos;
    [SerializeField] private float bobAmplitude;
    [SerializeField] private float bobFrequency;
    public float movementSpeed;
    public float nodeContactRadius;
    private bool walking;

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
        walking = false;

        foreach (var sprite in handSprites) 
        {
            sprite.color = Color.clear;
        }
        gunPivot.SetActive(true);
        currentFollowPoint = Vector2.zero;
        startPos = bodySpriteAnchor.localPosition;
    }

    private void Update()
    {
        currentFollowPoint = pathfindingNode.currentPath[pathfindingNode.pointsIndex];
        directionToPoint = currentFollowPoint - transform.position;

        //transform.position = Vector3.MoveTowards(transform.position, currentFollowPoint, movementSpeed * Time.deltaTime);

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

        walking = rb.velocity.magnitude > 0;
        if (walking)
        {
            bodySpriteAnchor.localPosition += FootStepMotion();
        }
        ResetPosition();
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        return pos;
    }

    private void ResetPosition()
    {
        if (bodySpriteAnchor.localPosition == startPos) return;

        bodySpriteAnchor.localPosition = Vector3.Lerp(bodySpriteAnchor.localPosition, startPos, 1 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = directionToPoint.normalized * movementSpeed;
    }
}
