using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundWeaponController : MonoBehaviour
{
    public GameObject gunPivot;
    public GameObject gunAnchor;
    public SpriteRenderer gunSprite;
    public GroundGuns groundGuns;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            groundGuns.Shoot();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       Vector3 pointDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
       pointDirection.Normalize();
       float rotationZ = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
       print(rotationZ);
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
