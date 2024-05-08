using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static SpawnPoint instance;

    public static SpawnPoint GetInstance() => instance;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        Player.GetInstance().gameObject.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }
}
