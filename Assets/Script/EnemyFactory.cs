using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    //[SerializeField] List<GameObject> weakEnemy = new();
    //[SerializeField] List<GameObject> strongEnemy = new();
    [SerializeField] GameObject weakEnemy;
    [SerializeField] GameObject strongEnemy;

    private static EnemyFactory instance;

    public static EnemyFactory GetInstance() => instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject CreateWeakEnemy() 
    {
        return ObjectPool.GetInstance().GetPooledObject(weakEnemy);
    }

    public GameObject CreateStrongEnemy() 
    {
        return ObjectPool.GetInstance().GetPooledObject(strongEnemy);
    }
}
