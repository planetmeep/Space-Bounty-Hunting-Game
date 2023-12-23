using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyguardWeaponController : MonoBehaviour
{
    public GameObject gunPivot;
    public GameObject gunAnchor;
    public SpriteRenderer gunSprite;
    public GroundGuns groundGuns;
    public Vector3 lookVector;

    private void Start()
    {
        lookVector = transform.right;
    }

    public void Fire()
    {
        float pointAngle = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
        groundGuns.Shoot(lookVector, Quaternion.Euler(0, 0, pointAngle - 90));
    }

    public void SetLookVector(Vector3 vector) 
    {
        lookVector = vector;
    }

    private void Update()
    {
        Vector3 pointDirection = lookVector;

        float rotationZ = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
        if (rotationZ < -90 || rotationZ > 90)
        {
            gunAnchor.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }
        else
        {
            gunAnchor.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        gunPivot.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}
