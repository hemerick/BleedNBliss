using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    //VARIABLES
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float range;
    [SerializeField] private int projectileCount = 3;
    private float attackSpeed = .75f;

    private float attackCooldown = 3;
    private float currentAttackCooldown = 0f;

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
        projectile.transform.SetPositionAndRotation(transform.position, rotation);
        projectile.SetActive(true);
        projectile.GetComponent<IPoolable>().Reset();

    }


}
