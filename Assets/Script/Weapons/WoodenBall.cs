using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class WoodenBall : Projectile, IUpdateProjectileStats
{
    public void SetProjectileStats(float newDamage, float newLifetime)
    {
        damage= newDamage;
        lifetime= newLifetime;
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
