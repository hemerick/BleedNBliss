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
    public static int[] xpValueRange= new int[] { 500, 100, 25, 5, 1};

    //COMPONENTS
    SpriteRenderer sprite;
    Rigidbody2D rb;

    private IExperienceObserver observer;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        observer = Player.GetInstance();
    }

    public void Reset()
    {
        xpValue= 0;
        currentLifetime = TOTAL_LIFETIME;
    }

    private void OnEnable()
    {
        Enemy.EnemyDeathEvent += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        Enemy.EnemyDeathEvent += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(int experienceDrop)
    {
        xpValue = experienceDrop;
        SetColorByValue();
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

    private void SetColorByValue() 
    {
        switch (xpValue) 
        {
            case 1:
            {
                    sprite.color = Color.white;
            }
            break;

            case 5:
            {
                    sprite.color = Color.blue;
            }
            break;

            case 25:
            {
                    sprite.color = Color.magenta;
            }
            break;

            case 100:
            {
                    sprite.color = Color.yellow;
            }
            break;

            case 500:
            {
                    sprite.color = Color.red;
            }
            break;

            default: 
            {
                    Debug.LogError("INCORRECT VALUE :" + xpValue);
            }
            break;
        }
    }
}
