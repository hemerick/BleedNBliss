using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IPoolable, IDealDamage
{
    //VARIABLES
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHealthPoint;
    [SerializeField] private int experienceDrop;
    [SerializeField] private int attackDamage;
    [SerializeField] private float range;
    [SerializeField] private int projectileCount = 3;
    private float attackSpeed = 3f;

    float AttackCooldown = 2;
    float currentAttackCooldown;

    private float healthPoint;
    private bool isDead = false;
    //CONSTANTES
    private const int EXP_DROP_CHANCE = 75;

    //COMPONENTS
    private GameObject target;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    //EVENT
    public static event Action<int> EnemyDeathEvent;
    private void Start()
    {
        target = Player.GetInstance().gameObject;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Reset()
    {
        healthPoint = maxHealthPoint;
        isDead = false;

        if (isActiveAndEnabled)
        {
            sprite.color = Color.white;
        }
    }

    private IEnumerator AttackSequence()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Shoot();

            yield return new WaitForSeconds(.095f);
        }
    }


    private void FixedUpdate()
    {
        if (!isDead)
        {

            if (InRange())
            {
                StopMovement();
                if (currentAttackCooldown <= 0)
                {
                    StartCoroutine(AttackSequence());
                    currentAttackCooldown = AttackCooldown;
                }
                else
                {
                    currentAttackCooldown -= Time.deltaTime * attackSpeed;
                }

            }
            else
            {
                StopCoroutine(AttackSequence());
                MoveTowardPlayer();
            }
        }
    }

    private void Shoot() 
    {
        //CALCULE LA DIRECTION DE LA CIBLE
        Vector3 directionToTarget = target.transform.position - transform.position;
        directionToTarget.Normalize();

        //CALCULE L'ANGLE DE ROTATION EN RADIANS
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //PREND UN PROJECTILE DU POOL ET L'ORIENTE VERS LA CIBLE
        GameObject projectile = ObjectPool.GetInstance().GetPooledObject(projectilePrefab);
        projectile.transform.SetPositionAndRotation(transform.position, rotation);
        projectile.SetActive(true);
        projectile.GetComponent<IPoolable>().Reset();

    }

    private bool InRange()
    {
        float distanceBetweenPlayer = Vector3.Distance(transform.position, target.transform.position);
        if (distanceBetweenPlayer <= range)
        {
            return true;
        }
        else { return false; }
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 10);
    }

    public void InflictDamage(float damage)
    {
        TakeDamage(damage);
    }

    //APPLIQUE X DÉGATS
    private void TakeDamage(float damage)
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
            if (isActiveAndEnabled)
            {
                StartCoroutine(FlashRed());
            }

        }
    }

    private void MoveTowardPlayer()
    {

        Vector2 direction = (target.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    }

    private bool CanDropExperience(float dropPercent)
    {
        float tempDropPercent = UnityEngine.Random.Range(0, 100);
        if (tempDropPercent <= dropPercent)
        {
            return true;
        }
        return false;
    }

    private void Death()
    {
        if (CanDropExperience(EXP_DROP_CHANCE))
        {
            SpawnExp();
        }
        StopAllCoroutines();
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }


    private List<int> CalculExpValue()
    {
        List<int> values = new();
        int remainingExp = experienceDrop;

        foreach (int exp in Experience.xpValueRange)
        {
            while (remainingExp >= exp)
            {
                values.Add(exp);
                remainingExp -= exp;
            }
        }

        return values;
    }

    private void SpawnExp()
    {
        foreach (int xpObj in CalculExpValue())
        {
            GameObject experience = ObjectPool.GetInstance().GetPooledObject(experiencePrefab);
            experience.transform.SetPositionAndRotation(RandomPositionAroundEnemy(), Quaternion.identity);
            experience.GetComponent<IPoolable>().Reset();
            experience.SetActive(true);

            //Debug.Log(xpObj);
            EnemyDeathEvent?.Invoke(xpObj);
        }

    }

    private Vector3 RandomPositionAroundEnemy()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * .25f;
        return new Vector3(transform.position.x + randomDirection.x, transform.position.y + randomDirection.y);
    }
}
