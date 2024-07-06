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

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(2.5f, 2.5f, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-2.5f, 2.5f, 1);
        }
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }
}
