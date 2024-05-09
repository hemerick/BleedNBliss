using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDealDamage
{
    void InflictDamage(float damage);
}

public class Projectile : MonoBehaviour, IPoolable, IDamage
{
    //private float damage;
    private float rotationSpeed = 575f;
    float lifetime = 0.4f;
    float damage;

    Vector3 movement;

    public void Reset()
    {
        lifetime = 0.4f;
        //movement = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector3.right;
        movement = transform.rotation * Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            gameObject.SetActive(false);
        }

        Movement();

    }


    void Movement()
    {
        //Mouvement
        transform.position += 12.5f * Time.deltaTime * movement;

        //Rotation sur lui-même
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
    }

    public void OnDamageChanged(float newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var hittedTarget = collision.GetComponent<IDealDamage>();
        hittedTarget?.InflictDamage(damage);

    }
}
