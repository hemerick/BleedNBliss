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
    private float range = 2.5f;

    //CONSTANTES
    [SerializeField] private const float TOTAL_LIFETIME = 10f;
    public static int[] xpValueRange= new int[] { 500, 100, 25, 5, 1};

    //COMPONENTS
    SpriteRenderer sprite;
    Rigidbody2D rb;
    GameObject player;

    private IExperienceObserver observer;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        observer = Player.GetInstance();
        player = Player.GetInstance().gameObject;
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
        Enemy.EnemyDeathEvent -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(int experienceDrop)
    {
        if(xpValue == 0) 
        {
            xpValue = experienceDrop;
            //Debug.Log("XP VALUE : " + xpValue);
            SetColorByValue();
        }
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

        if (InRange())
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
            SoundPlayer.GetInstance().PlayCollectingAudio();
            NotifyObserver();
            gameObject.SetActive(false);
        }
    }

    private bool InRange() 
    {
        float distanceBetweenPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceBetweenPlayer <= range)
        {
            return true;
        }
        else { return false; }
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
        if(observer != null && xpValue > 0)
        {
            observer.GainExperience(xpValue);
            xpValue= 0;
        }
        else
        {
            Debug.LogError("Exp Observer not set!!!");
        }
        //Debug.Log("XP : VALUE SENT -> " + xpValue);
        
    }

    private void SetColorByValue() 
    {
        Color color = xpValue switch
        {
            1 => Color.white,
            5 => Color.cyan,
            25 => Color.magenta,
            100 => Color.yellow,
            500 => Color.red,
            _ => Color.black //Default Color (Should't happend)
        };

        sprite.color = color;
        if(xpValue != 500 && xpValue != 100 && xpValue != 25 && xpValue != 5 && xpValue != 1)
        {
            Debug.LogError("Unexpected XP Value! : " + xpValue);
        }

    }
}
