using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    protected override void MoveTowardPlayer()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
}
