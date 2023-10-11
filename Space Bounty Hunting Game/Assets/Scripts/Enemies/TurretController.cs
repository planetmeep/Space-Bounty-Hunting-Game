using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : BaseEnemy
{
    public override void UpdateBehavior() 
    { 
        CheckAndAttackPlayer();
    }
}
