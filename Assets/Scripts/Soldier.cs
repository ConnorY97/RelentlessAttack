using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : EnemyBase
{
    public override void Init(int hitPoints, GameObject target, bool isEnemy, float speed)
    {
        base.Init(hitPoints, target, isEnemy, speed);
    }

    public override void Tick()
    {
        base.Tick();
    }
}
