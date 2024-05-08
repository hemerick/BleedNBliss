using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour, IExperienceObserver
{
    // VARIABLES
    [SerializeField] GameObject scythePrefab;
    List<GameObject> targets = new();
    Rigidbody2D rb;
    SpriteRenderer sprite;

    private static Player instance;

    public static Player GetInstance() => instance;

    private float moveSpeed = 5f;
    private float attackSpeed = 2f;
    private float attackDamage;
    private int maxHealthPoint = 10;
    private int healthPoint = 10;
    private int projectileCount = 1;

    public int playerLVL = 1;
    public int playerXP = 0;
    public int RequiredXp = 8;

    public bool isDead = false;
    float AttackCooldown = 2;
    float currentAttackCooldown;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);
        GameManager.GetInstance().SetPlayerXPDisplay(playerXP, RequiredXp);
        GameManager.GetInstance().SetCurrentLvLDisplay(playerLVL);
    }

    private void Update()
    {
        if (currentAttackCooldown <= 0 && targets.Count > 0)
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
        GameObject targetToAttack = ClosestTarget();

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

    }

    //DÉTERMINE L'ENEMY LE PLUS PROCHE
    private GameObject ClosestTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;


        //POUR CHAQUE ENEMY DANS LA LISTE DE TARGET, COMPARE LA DISTANCE
        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        //RETOURNE L'ENEMY LE PLUS PROCHE DU JOUEUR
        return closestTarget;
    }

    //AJOUTE LES ENEMY IN-RANGE DANS LA LISTE DE TARGET
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            targets.Add(trigger.gameObject);
        }

    }

    //RETIRE LES ENEMY OUT OF RANGE DE LA LISTE DE TARGET
    private void OnTriggerExit2D(Collider2D trigger)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            targets.Remove(trigger.gameObject);
        }
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
        playerLVL++;
        projectileCount = playerLVL/2;
        GameManager.GetInstance().SetPlayerHPDisplay(healthPoint, maxHealthPoint);
        GameManager.GetInstance().SetCurrentLvLDisplay(playerLVL);
    }

}
