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
    private float healthPoint = 10f;

    public int playerXP = 0;

    bool isDead= false;
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
    }

    private void Update()
    {
        if (currentAttackCooldown <= 0 && targets.Count > 0)
        {
            AttackClosestEnemy();
            currentAttackCooldown = AttackCooldown;
        }
        else
        {
            currentAttackCooldown -= Time.deltaTime * attackSpeed;
        }
    }


    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed;

    }

    //FAIT APPARAITRE UN PROJECTILE ORIENTÉ DANS LA DIRECTION DE L'ENEMY LE PLUS PROCHE
    private void AttackClosestEnemy()
    {
        //DÉFINI LA CIBLE A ATTACK
        GameObject targetToAttack = ClosestTarget();

        if (targetToAttack == null) { return; }

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
        if(collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void TakeDamage(int damage)
    {
        healthPoint -= damage;

        if (healthPoint <= 0 && !isDead)
        {
            isDead = true;

            SoundPlayer.GetInstance().PlayFinalDamageAudio();

            Death();
        }
        else
        {
            SoundPlayer.GetInstance().PlayDamageAudio();
            StartCoroutine(FlashRed());
        }
    }

    private void Death()
    {
        Debug.Log("DEAD!");
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = Color.white;
    }

    public void GainExperience(int xpValue)
    {
        Debug.Log("EXP RECEIVED : " + xpValue);
        playerXP += xpValue;
        Debug.Log("PLAYER XP : " + playerXP);

        ExperienceBar.GetInstance().SetExperience(playerXP);
    }
}
