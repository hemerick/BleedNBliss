using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //private float damage;
    private float rotationSpeed = 575f;
    Vector3 movement;

    private void Start()
    {
        movement = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += movement * 12.5f * Time.deltaTime;

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

}
