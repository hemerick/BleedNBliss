using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : MonoBehaviour, IPoolable
{
    //VARIABLES
    Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float healthPoint = 3f;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Reset()
    {
        healthPoint = 3f;
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
            takeDamage(1f);
        }

    }

    //APPLIQUE X DÉGATS A MERMAN
    private void takeDamage(float damage)
    {
        healthPoint -= damage;

        if (healthPoint <= 0 && !isDead)
        {
            isDead = true;

            SoundPlayer.GetInstance().PlayDeathAudio();

            Death();
        }
        else
        {
            SoundPlayer.GetInstance().PlayHurtAudio();
            //Debug.Log(name + "| HP : " + healthPoint);
        }
    }

    private void MoveTowardPlayer()
    {

        Vector2 direction = (Player.GetInstance().transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

    }

    private void Death()
    {
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }

}
