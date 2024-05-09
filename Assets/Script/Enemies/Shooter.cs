using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateProjectileStats
{
    void SetProjectileStats(float damage);
}

public class Shooter : Enemy
{
    //VARIABLES
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float range;
    [SerializeField] private int projectileCount = 3;
    [SerializeField] private float attackSpeed = 1;

    private float attackCooldown = 2;
    private float currentAttackCooldown = 0f;


    private List<IUpdateProjectileStats> damageObserver = new();


    //OBSERVER
    public void RegisterDamage(IUpdateProjectileStats observer)
    {
        if (!damageObserver.Contains(observer))
        {
            damageObserver.Add(observer);
            observer.SetProjectileStats(attackDamage);
        }
    }

    public void UnRegisterDamage(IUpdateProjectileStats observer)
    {
        if (damageObserver.Contains(observer))
        {
            damageObserver.Remove(observer);
        }
    }

    private void NotifyDamageObservers()
    {
        foreach (var observer in damageObserver)
        {
            observer.SetProjectileStats(attackDamage);
        }
    }


    protected override void Start()
    {
        base.Start(); //Appel la fonction de base dans Enemy
        currentAttackCooldown= attackCooldown;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isDead)
        {
            if (InRange())
            {
                StopMovement();
                if (currentAttackCooldown <= 0)
                {
                    StartCoroutine(AttackSequence());
                    currentAttackCooldown = attackCooldown;
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
    protected override void MoveTowardPlayer()
    {
        if(!InRange())
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }
    private void StopMovement()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 10);
    }
    private IEnumerator AttackSequence()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Shoot();

            yield return new WaitForSeconds(.15f);
        }
    }

    private bool InRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= range;
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
        projectile.GetComponent<Projectile>().source = ProjectileSource.Enemy;
        projectile.transform.SetPositionAndRotation(transform.position, rotation);
        projectile.SetActive(true);
        projectile.GetComponent<IPoolable>().Reset();

        RegisterDamage(projectile.GetComponent<WoodenBall>());

    }

}
