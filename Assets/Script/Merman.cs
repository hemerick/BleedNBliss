using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : MonoBehaviour, IPoolable
{
    //VARIABLES
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float healthPoint = 3f;
    private GameObject target;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool isDead = false;

    private void Start()
    {
        target = Player.GetInstance().gameObject;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
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
            StartCoroutine(FlashRed());
            //Debug.Log(name + "| HP : " + healthPoint);
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

    private void Death()
    {
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }

}
