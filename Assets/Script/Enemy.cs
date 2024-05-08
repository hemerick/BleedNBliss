using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.UI;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    //VARIABLES
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float maxHealthPoint;
    [SerializeField] private int experienceDrop;

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
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            MoveTowardPlayer();
        }
    }


    //VÉRIFIE LES COLLISIONS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SI LA COLLISION EST UN PROJECTILE
        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(1f);                                    // À CHANGER !!!!!!!! ---> WEAPON INFLICT DAMAGE
        }

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
            if(isActiveAndEnabled) 
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
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }


    private List<int> CalculExpValue() 
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

    private void SpawnExp() 
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

    private Vector3 RandomPositionAroundEnemy()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * .25f;
        return new Vector3(transform.position.x + randomDirection.x, transform.position.y + randomDirection.y);
    }
}
