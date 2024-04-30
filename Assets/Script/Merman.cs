using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merman : MonoBehaviour
{
    //VARIABLES
    Rigidbody2D rb;
    private float healthPoint = 3f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        healthPoint--;
        if (isDead(healthPoint))
        {
            Destroy(gameObject);
        }
        else 
        {
            print(healthPoint);
        }
    }

    private bool isDead(float healthPoint)
    { 
        if(healthPoint <= 0) 
        {
            return true;
        }
        else { return false; }
    }
}
