using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    // VARIABLES
    [SerializeField] GameObject scythePrefab;
    Rigidbody2D rb;
    private float moveSpeed = 5f;
    //private float attackSpeed = 1f;
    //private float attackDamage;
    //private float healthPoint = 10f;

    float AttackCooldown = 2;
    float currentAttackCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //InvokeRepeating("Attack", 0 ,attackSpeed); //pour appeler la fonction attack
    }

    private void Update()
    {
        currentAttackCooldown -= Time.deltaTime; // * attackSpeed;
        if(currentAttackCooldown <= 0) 
        {
            Attack();
        }
    }


    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //Méthode Altérnative pour le déplacement du joueur
        //Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        //transform.Translate(movement * moveSpeed * Time.deltaTime);

        rb.velocity = new Vector2(horizontalInput, verticalInput) * moveSpeed;

    }

    private void Attack() 
    {
        Instantiate(scythePrefab, transform.position, Quaternion.identity); //Quaternion.identity laisse la rotation à 0
        currentAttackCooldown += AttackCooldown;
    }

}
