using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    //private float damage;
    private float rotationSpeed = 575f;
    float lifetime = 0.4f;

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
        if(lifetime < 0 )
        {
            gameObject.SetActive(false);
        }
        //Mouvement
        transform.position += movement * 12.5f * Time.deltaTime;

        //Rotation sur lui-même
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

}
