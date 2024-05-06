using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Experience : MonoBehaviour, IPoolable
{
    Rigidbody2D rb;
    bool isInRange = false;
    private int xpValue;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Reset()
    {
        xpValue = Random.Range(1, 5);
    }

    private void Update()
    {
        if (isInRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            StopMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange= true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            isInRange= false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoundPlayer.GetInstance().PlayCollectingAudio();
            gameObject.SetActive(false);
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = (Player.GetInstance().transform.position - transform.position).normalized;
        rb.velocity = direction * 2.5f;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }
}
