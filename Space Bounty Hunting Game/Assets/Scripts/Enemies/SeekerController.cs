using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : BaseEnemy
{
    public override void UpdateBehavior() 
    { 
        FacePlayer();
        bool withinRange = CheckPlayerWithinWeaponsRange();
        if(withinRange)
        {
            AttackPlayer(playerTransform);
        }
        else
        {
            MoveTowardsPlayer();
        }

    }
    
}
