using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

public interface IAttackPlayer 
{
    public void AttackPlayer(int damage);
}

public abstract class Enemy : MonoBehaviour, IPoolable, IWeaponDamage
{
    //VARIABLES
    [SerializeField] protected GameObject experiencePrefab;
    [SerializeField] protected float maxHealthPoint;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected int experienceDrop;

    protected float healthPoint;
    protected bool isDead = false;

    //CONSTANTES
    protected const int EXP_DROP_CHANCE = 75;

    //COMPONENTS
    protected GameObject target;
    protected Rigidbody2D rb;
    protected SpriteRenderer sprite;

    //EVENT
    public static event Action<int> EnemyDeathEvent;
    

    protected virtual void Start()
    {
        target = Player.GetInstance().gameObject;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Reset()
    {
        healthPoint = maxHealthPoint;
        isDead = false;

        if(isActiveAndEnabled) 
        {
            sprite.color = Color.white;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isDead)
        {
            MoveTowardPlayer();
        }
    }

    //VÉRIFIE LES COLLISIONS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hittedTarget = collision.GetComponent<IAttackPlayer>();
        hittedTarget?.AttackPlayer(attackDamage);
    }
    public void ProjectileInflictDamage(float damage)
    {
        TakeDamage(damage);
    }

    //RECOIT X DÉGATS
    protected virtual void TakeDamage(float damage)
    {
        healthPoint -= damage;

        if (healthPoint <= 0 && !isDead)
        {
            isDead = true;

            SoundPlayer.GetInstance().PlayDeathAudio();
            StopCoroutine(FlashRed());

            Death();
        }
        else
        {
            SoundPlayer.GetInstance().PlayHurtAudio();
            if(isActiveAndEnabled) 
            {
                StartCoroutine(FlashRed());
            }
            
        }
    }

    protected abstract void MoveTowardPlayer();
    //{

    //    Vector2 direction = (target.transform.position - transform.position).normalized;
    //    rb.velocity = direction * moveSpeed;

    //} 

    protected IEnumerator FlashRed() 
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    }

    protected bool CanDropExperience(float dropPercent) 
    {
        float tempDropPercent = UnityEngine.Random.Range(0, 100);
        return tempDropPercent <= dropPercent;
    }

    protected virtual void Death()
    {
        if (CanDropExperience(EXP_DROP_CHANCE))
        {
            SpawnExp();
        }
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }


    protected List<int> CalculExpValue() 
    {
        List<int> values = new();
        int remainingExp = experienceDrop;

        foreach(int exp in Experience.xpValueRange) 
        {
            while(remainingExp >= exp) 
            {
                values.Add(exp);
                remainingExp -= exp;
            }
        }

        return values;
    }

    protected void SpawnExp() 
    {
        foreach(int xpObj in CalculExpValue())
        {
            GameObject experience = ObjectPool.GetInstance().GetPooledObject(experiencePrefab);
            experience.transform.SetPositionAndRotation(RandomPositionAroundEnemy(), Quaternion.identity);
            experience.GetComponent<IPoolable>().Reset();
            experience.SetActive(true);

            //Debug.Log(xpObj);
            EnemyDeathEvent?.Invoke(xpObj);
        }
            
    }

    protected Vector3 RandomPositionAroundEnemy()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * .25f;
        return new Vector3(transform.position.x + randomDirection.x, transform.position.y + randomDirection.y);
    }

}
