using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 shotDirection;
    private float shotSpeed;

    public void SetupBullet(Vector3 shotDirection, float shotSpeed)
    {
        this.shotDirection = shotDirection;
        this.shotSpeed = shotSpeed;
    }

    private void Update()
    {
        transform.position += shotDirection * shotSpeed * Time.deltaTime;
    }
}