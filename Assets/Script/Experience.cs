using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IExperienceObserver
{
    public void GainExperience(int xpValue);
}


public class Experience : MonoBehaviour, IPoolable
{
    //VARIABLES
    private int xpValue;
    private float currentLifetime = 10f;
    private bool isInRange = false;

    //CONSTANTES
    [SerializeField] private const float TOTAL_LIFETIME = 10f;

    //COMPONENTS
    SpriteRenderer sprite;
    Rigidbody2D rb;

    private IExperienceObserver observer;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        observer = Player.GetInstance();
    }

    public void Reset()
    {
        currentLifetime = TOTAL_LIFETIME;
        xpValue = Random.Range(1, 5);
    }

    private void Update()
    {
        if(currentLifetime > 0) 
        {
            LifeChrono();
        }
        else 
        {
            gameObject.SetActive(false);
        }

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
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoundPlayer.GetInstance().PlayCollectingAudio();
            NotifyObserver();
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
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 10);
    }

    private void LifeChrono()
    {
        currentLifetime -= Time.deltaTime;

        float alpha = currentLifetime / TOTAL_LIFETIME;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
    }

    //OBSERVER

    private void NotifyObserver()
    {
        observer.GainExperience(xpValue);
    }
}
