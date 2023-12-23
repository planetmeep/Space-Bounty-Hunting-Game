using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterWeaponController : MonoBehaviour
{
    public ShipGuns shipGuns;
    public Vector3 lookVector;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        lookVector = transform.up;
    }

    public void Fire()
    {
        shipGuns.Shoot();
    }

    public void SetLookVector(Vector3 vector)
    {
        lookVector = vector;
    }

    private void Update()
    {
        Vector3 pointDirection = lookVector;
        float rotationZ = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;
        rb.MoveRotation(Quaternion.Euler(0f, 0f, rotationZ - 90));
    }
}
