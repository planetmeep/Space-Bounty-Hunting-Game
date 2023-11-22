using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BodyguardWeaponController : MonoBehaviour
{
    public GameObject gunPivot;
    public GameObject gunAnchor;
    public SpriteRenderer gunSprite;
    public GroundGuns groundGuns;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            ShootTowardsPlayer();
        }
    }

    private void ShootTowardsPlayer()
    {
        Vector3 pointDirection = player.position - transform.position;
        float pointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
        groundGuns.Shoot(pointDirection, Quaternion.Euler(0, 0, pointAngle - 90));
    }

    private void FixedUpdate()
    {
        Vector3 pointDirection = player.position - transform.position;
        pointDirection.Normalize();
        float rotationZ = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
        if (rotationZ < -90 || rotationZ > 90)
        {
            //gunSprite.flipY = true;
            gunAnchor.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }
        else
        {
            //gunSprite.flipY = false;
            gunAnchor.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        gunPivot.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}
