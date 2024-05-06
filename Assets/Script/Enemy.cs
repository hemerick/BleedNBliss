using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Merman : MonoBehaviour, IPoolable
{
    //VARIABLES
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float healthPoint = 3f;
    [SerializeField] private int experienceDrop;
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


    //V�RIFIE LES COLLISIONS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SI LA COLLISION EST UN PROJECTILE
        if (collision.gameObject.CompareTag("Projectile"))
        {
            takeDamage(1f);
        }

    }

    //APPLIQUE X D�GATS A MERMAN
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

    private bool dropExperience(float dropPercent) 
    {
        float tempDropPercent = Random.Range(0, 100);
        if (tempDropPercent <= dropPercent) 
        {
            return true;
        }
        return false;
    }

    private void Death()
    {
        if (dropExperience(50)) 
        {
            GameObject experience = ObjectPool.GetInstance().GetPooledObject(experiencePrefab);
            experience.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            experience.GetComponent<IPoolable>().Reset();
            experience.SetActive(true);
        }
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }

}
