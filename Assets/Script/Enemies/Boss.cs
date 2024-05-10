using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
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
        boss.StopMovement();
    }

    public override void Exit()
    {
        boss.StopMovement();
    }

    public override void Update()
    {
        boss.Movement(true, boss.Speed);
    }
}

public class DashAtPlayerState : BossState
{
    public DashAtPlayerState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.StopMovement();
    }

    public override void Exit()
    {
        boss.StopMovement();
    }

    public override void Update()
    {
        boss.Movement(true, 0);    //DOESN'T MOVE - JUST LOOK AT PLAYER
        Debug.Log("NOT IMPLEMENTED YET");
    }
}

public class RunAwayFromPlayerState : BossState
{
    public RunAwayFromPlayerState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.StopMovement();
    }

    public override void Exit()
    {
        boss.StopMovement();
    }

    public override void Update()
    {
        boss.Movement(false, boss.Speed);
    }
}

public class AttackPlayerState : BossState
{
    public AttackPlayerState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.StopMovement();
    }

    public override void Exit() 
    {
        boss.StopMovement();
    }

    public override void Update()
    {
        boss.Movement(true, 0);    //DOESN'T MOVE - JUST LOOK AT PLAYER
        Debug.Log("NO ATTACK YET");
    }
}

public class DyingState : BossState
{
    public DyingState(Boss boss) : base(boss) {}

    public override void Enter()
    {
        boss.DeathAnimation();
    }

    public override void Exit() {}

    public override void Update() {}
}

public class Boss : MonoBehaviour, IPoolable, IWeaponDamage
{
    private Animator animator;
    private GameObject target;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    //BOSS STATS
    [SerializeField] public float Speed = 3f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float maxHealthPoint = 100;
    [SerializeField] private float attackRange = 5f;

    Vector3 direction;
    float healthPoint = 100;
    bool isDead = false;

    //CONSTANTES
    private float[] HP_TRIGGER_PERCENT = new float[] { 75f, 50f, 25f};

    private BossState currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        target = Player.GetInstance().gameObject;
        sprite = GetComponent<SpriteRenderer>();

        SetState(CheckHP());
    }

    private void Update()
    {
        currentState?.Update();
    }
    public void Reset()
    {
        healthPoint = maxHealthPoint;
        isDead = false;
    }

    //STATES
    public void SetState(BossState state)
    {
        if(currentState == state) return;
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    //METHODES (mettre public pour être accessible dans les StateClass)
    //Implémenter la logique ici
    public void Movement(bool followPlayer, float moveSpeed) 
    {
        if(followPlayer) 
        {
            direction = (target.transform.position - transform.position).normalized;
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
        else if(!followPlayer)
        {
            direction = (transform.position - target.transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(2.5f, 2.5f, 1);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-2.5f, 2.5f, 1);
            }
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", -direction.y);
        }
        
    } //Le bool signifie Poursuite ou Fuite
    
    public void StopMovement() 
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 10);
    }

    public void DashAtPlayer() { }

    public void ShootAtPlayer() { }

    private void TakeDamage(float damageReceived) 
    {
        healthPoint -= damageReceived;

        if (healthPoint <= 0 && !isDead)
        {
            isDead = true;

            SoundPlayer.GetInstance().PlayDeathAudio();
            StopCoroutine(FlashRed());

            SetState(CheckHP());
        }
        else
        {
            SoundPlayer.GetInstance().PlayHurtAudio();
            if (isActiveAndEnabled)
            {
                StartCoroutine(FlashRed());
            }
            SetState(CheckHP());
        }
    }

    private BossState CheckHP()
    {
        float healthPercent = (healthPoint / maxHealthPoint)*100;
        Debug.Log("HP % : " + healthPercent);

        if(healthPercent <= 0) {return new DyingState(this);}
        else if(healthPercent <= HP_TRIGGER_PERCENT[2]) { return new RunAwayFromPlayerState(this); }
        else if(healthPercent <= HP_TRIGGER_PERCENT[1]) { return new DashAtPlayerState(this); }
        else if(healthPercent <= HP_TRIGGER_PERCENT[0]) { return new AttackPlayerState(this); }
        else { return new ChasePlayerState(this);}
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    }

    public void DeathAnimation() 
    {
        rb.velocity= Vector3.zero;

        animator.SetBool("isDead", isDead);
    }

    public void DisableBoss()
    {
        gameObject.SetActive(false);
    }

    public void ProjectileInflictDamage(float damageReceived)
    {
        if(healthPoint <= 0) { return;}
        TakeDamage(damageReceived);
    } //FUNCTION TO RECEIVE DAMAGE FROM PLAYER PROJECTILE

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hittedTarget = collision.GetComponent<IAttackPlayer>();
        hittedTarget?.AttackPlayer(attackDamage);
    }
}