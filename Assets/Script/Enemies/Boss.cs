using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator animator;
    private GameObject target;
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 3f;

    Vector3 direction;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        target = Player.GetInstance().gameObject;
    }

    private void Update()
    {
        Movement();
    }

    private void Movement() 
    {
        direction = (target.transform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        if(direction.x < 0) 
        {
            transform.localScale = new Vector3(2.5f, 2.5f, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-2.5f, 2.5f, 1);
        }
    }
}
