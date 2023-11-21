using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    void OnHit(Projectile projectile, Vector2 hitPoint, Quaternion hitDirection);
}
