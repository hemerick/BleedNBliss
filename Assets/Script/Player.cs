using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    // VARIABLES
    [SerializeField] GameObject scythePrefab;
    List<GameObject> targets = new List<GameObject>();
    Rigidbody2D rb;
    CapsuleCollider2D playerRange;

    private float moveSpeed = 5f;
    private float attackSpeed = 2f;
    //private float attackDamage;
    //private float healthPoint = 10f;

    float AttackCooldown = 2;
    float currentAttackCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRange= GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        currentAttackCooldown -= Time.deltaTime * attackSpeed;
        if(currentAttackCooldown <= 0 && targets.Count > 0) 
        {
            AttackClosestEnemy();
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

        if (targetToAttack == null) { return;}

        //CALCULE LA DIRECTION DE LA CIBLE
        Vector3 directionToTarget = targetToAttack.transform.position - transform.position;
        directionToTarget.Normalize();

        //CALCULE L'ANGLE DE ROTATION EN RADIANS
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //PREND UN PROJECTILE DU POOL ET L'ORIENTE VERS LA CIBLE
        GameObject scythe = ObjectPool.GetInstance().GetPooledObject();
        scythe.transform.SetPositionAndRotation(transform.position, rotation);
        scythe.SetActive(true);
        scythe.GetComponent<IPoolable>().Reset();

        currentAttackCooldown += AttackCooldown; //COOLDOWN
    }

    //DÉTERMINE L'ENEMY LE PLUS PROCHE
    private GameObject ClosestTarget() 
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;


        //POUR CHAQUE ENEMY DANS LA LISTE DE TARGET, COMPARE LA DISTANCE
        foreach(GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        //RETOURNE L'ENEMY LE PLUS PROCHE DU JOUEUR
        return closestTarget;
    }

    //AJOUTE LES ENEMY IN-RANGE DANS LA LISTE DE TARGET
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if(collision.gameObject.CompareTag("Enemy")) 
        {
            targets.Add(collision.gameObject);
        }
        
    }

    //RETIRE LES ENEMY OUT OF RANGE DE LA LISTE DE TARGET
    private void OnTriggerExit2D(Collider2D collision)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if(collision.gameObject.CompareTag("Enemy"))
        {
            targets.Remove(collision.gameObject);
        }
    }

}
