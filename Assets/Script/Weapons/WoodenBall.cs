using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenBall : Projectile, IUpdateProjectileStats
{
    public void SetProjectileStats(float newDamage)
    {
        damage= newDamage;
    }

    protected override void CustomReset()
    {
        currentLifetime = lifetime;
    }

    protected override void Movement()
    {
        transform.position += moveSpeed * Time.deltaTime * movement;
    }
}
