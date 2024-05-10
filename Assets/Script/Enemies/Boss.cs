using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class BossState
{
    protected Boss boss;

    public BossState(Boss boss)
    {
        this.boss = boss;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

public class ChasePlayerState : BossState
{
    public ChasePlayerState(Boss boss) : base(boss) { }
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class DashAtPlayerState : BossState
{
    public DashAtPlayerState(Boss boss) : base(boss)
    {
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class RunAwayFromPlayerState : BossState
{
    public RunAwayFromPlayerState(Boss boss) : base(boss)
    {
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class AttackPlayerState : BossState
{
    public AttackPlayerState(Boss boss) : base(boss)
    {
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class DyingState : BossState
{
    public DyingState(Boss boss) : base(boss)
    {
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class Boss : MonoBehaviour, IPoolable
{
    private Animator animator;
    private GameObject target;
    private Rigidbody2D rb;

    //BOSS STATS
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int damage = 5;
    [SerializeField] private int maxHealthPoint = 1500;
    [SerializeField] private float attackRange = 5f;

    Vector3 direction;
    int healthPoint = 1500;
    bool isDead = false;

    //CONSTANTES
    private float[] HP_TRIGGER_PERCENT = new float[] { 75f, 50f, 25f, 0f };

    private BossState currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        target = Player.GetInstance().gameObject;
    }

    private void Update()
    {
        currentState?.Update();
        CheckTransition();
    }
    public void Reset()
    {
        healthPoint = maxHealthPoint;
        isDead = false;
    }

    //STATES
    public void SetState(BossState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    private void CheckTransition()
    {
        float healthPercent = healthPoint/maxHealthPoint;
        foreach (int hpTrigger in HP_TRIGGER_PERCENT)
        {
            while (healthPercent >= hpTrigger)
            {
                //Alternative?
                //EXEMPLE DE SETSTATE
                //SetState(new DyingState(this))
            }
        }
    }

    //METHODES (mettre public pour être accessible dans les StateClass)
    //Implémenter la logique ici
    public void Movement(bool followPlayer) 
    {
        if(followPlayer) 
        {
            direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else if(!followPlayer)
        {
            direction = (transform.position - target.transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        if(direction.x < 0) 
        {
            transform.localScale = new Vector3(2.5f, 2.5f, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-2.5f, 2.5f, 1);
        }
    } //Le bool signifie Poursuite ou Fuite

    public void DashAtPlayer() { }

    public void ShootAtPlayer() { }

    public void Death() { }

}
