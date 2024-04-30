using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : MonoBehaviour
{
    //VARIABLES
    Rigidbody2D rb;
    [SerializeField] private AudioClip soundClip;
    private AudioSource audioSource;
    private float healthPoint = 3f;
    private bool isDead = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = soundClip;
    }

    //VÉRIFIE LES COLLISIONS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SI LA COLLISION EST UN PROJECTILE
        if(collision.gameObject.CompareTag("Projectile"))
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

            audioSource.pitch = 0.6f;
            audioSource.Play();

            Invoke("Death", 0.15f);
        }
        else
        {
            audioSource.Play();
            Debug.Log(name + "| HP : " + healthPoint);
        }
    }

    private void Death() 
    {
        Destroy(gameObject);
    }

}
