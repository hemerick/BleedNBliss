using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : MonoBehaviour
{
    //VARIABLES
    SoundPlayer soundPlayer;
    Rigidbody2D rb;
    private float healthPoint = 3f;
    private bool isDead = false;


    private void Awake()
    {
        soundPlayer = SoundPlayer.GetInstance();
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

            soundPlayer.PlayDeathAudio();

            Death();
        }
        else
        {
            soundPlayer.PlayHurtAudio();
            Debug.Log(name + "| HP : " + healthPoint);
        }
    }

    private void Death() 
    {
        Destroy(gameObject);
    }

}
