using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * 5f * Time.deltaTime;
    }
}
