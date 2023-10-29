using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : BaseEnemy
{
    /*public override void UpdateBehavior() 
    { 
        FacePlayer();
        bool withinRange = CheckPlayerWithinWeaponsRange();
        if(withinRange)
        {
            AttackPlayer(playerTransform);
            engines.ApplyBrakes();
        }
        else
        {
            MoveTowardsPlayer();
        }

    }*/

    private void FixedUpdate()
    {
        bool withinWeaponRange = CheckPlayerWithinWeaponsRange();
        bool withinDetectionRange = CheckPlayerWithinDetectionRange();
        bool outsideFollowRadius = CheckPlayerOutsideStopFollow();

        if (withinWeaponRange)
        {
            FacePlayer();
            AttackPlayer(playerTransform);
            engines.ApplyBrakes();
        }
        if (withinDetectionRange && outsideFollowRadius)
        {
            FacePlayer();
            MoveTowardsPlayer();
        } 
        else 
        {
            engines.ApplyBrakes();
        }
    }
}
