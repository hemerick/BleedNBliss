using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;

public interface IDamage
{
    void OnDamageChanged(float damage);
}

public class Player : MonoBehaviour, IExperienceObserver
{
    // VARIABLES
    [SerializeField] GameObject scythePrefab;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    PlayerRange range;

    private static Player instance;

    public static Player GetInstance() => instance;

    //PLAYER STATS
    private float moveSpeed = 5f;
    private float attackSpeed = 2f;
    private float attackDamage = 1f;
    private int maxHealthPoint = 10;
    private int projectileCount = 1;

    public int playerLVL = 1;
    public int playerXP = 0;
    public int RequiredXp = 8;

    public bool isDead = false;
    private int healthPoint = 10;
    float AttackCooldown = 2;
    float currentAttackCooldown;

    private List<IDamage> damageObserver = new();


    //OBSERVER
    public void RegisterDamage(IDamage observer)
    {
        if (!damageObserver.Contains(observer)) 
        {
            damageObserver.Add(observer);
            observer.OnDamageChanged(attackDamage);
        }
    }

    public void UnRegisterDamage(IDamage observer) 
    {
        if(damageObserver.Contains(observer)) 
        {
            damageObserver.Remove(observer);
        }
    }

    private void NotifyDamageObservers()
    {
        foreach (var observer in damageObserver) 
        {
            observer.OnDamageChanged(attackDamage);
        }
    }


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        range = GetComponentInChildren<PlayerRange>();

        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);
        GameManager.GetInstance().SetPlayerXPDisplay(playerXP, RequiredXp);
        GameManager.GetInstance().SetCurrentLvLDisplay(playerLVL);
    }

    private void Update()
    {
        if (currentAttackCooldown <= 0 && range.targets.Count > 0)
        {
            StartCoroutine(AttackSequence());
            currentAttackCooldown = AttackCooldown;
        }
        else
        {
            currentAttackCooldown -= Time.deltaTime * attackSpeed;
        }
    }

    //MOVEMENT
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed;

    }

    private IEnumerator AttackSequence() 
    {
        for (int i = 0; i < projectileCount; i++)
        {
            AttackClosestEnemy();

            yield return new WaitForSeconds(.095f);
        }
    }


    //FAIT APPARAITRE UN PROJECTILE ORIENTÉ DANS LA DIRECTION DE L'ENEMY LE PLUS PROCHE
    private void AttackClosestEnemy()
    {
        //DÉFINI LA CIBLE A ATTACK
        GameObject targetToAttack = range.ClosestTarget();

        if (targetToAttack == null || !targetToAttack.activeInHierarchy) { return; }

        //CALCULE LA DIRECTION DE LA CIBLE
        Vector3 directionToTarget = targetToAttack.transform.position - transform.position;
        directionToTarget.Normalize();

        //CALCULE L'ANGLE DE ROTATION EN RADIANS
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //PREND UN PROJECTILE DU POOL ET L'ORIENTE VERS LA CIBLE
        GameObject scythe = ObjectPool.GetInstance().GetPooledObject(scythePrefab);
        scythe.transform.SetPositionAndRotation(transform.position, rotation);
        scythe.SetActive(true);
        scythe.GetComponent<IPoolable>().Reset();

        RegisterDamage(scythe.GetComponent<Projectile>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int damage)
    {
        healthPoint -= damage;
        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);

        if (healthPoint <= 0 && !isDead)
        {
            isDead = true;

            SoundPlayer.GetInstance().PlayFinalDamageAudio();
            StopCoroutine(FlashRed());

            Death();
        }
        else
        {
            SoundPlayer.GetInstance().PlayDamageAudio();
            if (isActiveAndEnabled)
            {
                StartCoroutine(FlashRed());
            }
        }
    }

    private void Death()
    {
        GameManager.GetInstance().PlayerDeath();
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        isDead = false;
        maxHealthPoint = 10;
        healthPoint = maxHealthPoint;
        projectileCount= 1;
        playerXP = 0;
        RequiredXp = 8;
        playerLVL = 1;
        GameManager.GetInstance().SetPlayerXPDisplay(playerXP, RequiredXp);
        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);
        GameManager.GetInstance().SetCurrentLvLDisplay(playerLVL);
        sprite.color = Color.white;
        gameObject.SetActive(true);
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    }

    public void GainExperience(int xpValue)                 // <----------- BUG HERE (XP GAIN IS ALWAYS 1 FOR SOME REASONS)
    {
        //Debug.Log("EXP RECEIVED : " + xpValue);
        playerXP += xpValue;
        //Debug.Log("PLAYER XP : " + playerXP);

        if (playerXP >= RequiredXp)
        {
            SoundPlayer.GetInstance().PlayLevelUpAudio();
            playerXP -= RequiredXp;
            RequiredXp *= 2;
            LevelUp();
        }
        GameManager.GetInstance().SetPlayerXPDisplay(playerXP, RequiredXp);
    }

    private void LevelUp()
    {
        maxHealthPoint += 1 * (playerLVL/3);
        healthPoint += 1 * (playerLVL/3);
        attackSpeed += .5f;
        moveSpeed += .1f;
        attackDamage += .5f;
        NotifyDamageObservers();
        playerLVL++;
        projectileCount = playerLVL/2;
        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);
        GameManager.GetInstance().SetCurrentLvLDisplay(playerLVL);
    }

}
