using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

//[Serializable]
//public class PoolInfo
//{
//    GameObject objectToPool;
//    public int poolCount = 100;
//}


public interface IPoolable 
{
    void Reset();
}

public class ObjectPool : MonoBehaviour
{
    //[SerializeField] PoolInfo poolInfo;
    [SerializeField] GameObject objectToPool;
    [SerializeField] int poolCount = 100;

    List<GameObject> pooledObjects = new();

    private static ObjectPool instance;

    public static ObjectPool GetInstance() => instance;
    int poolIndex;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < poolCount; i++)
        {
            GameObject g = Instantiate(objectToPool, Vector3.zero, Quaternion.identity, transform); //Le dernier paramètre (transfrom) sert a définir ObjectPool comme parent de l'object créer
            g.SetActive(false);
            pooledObjects.Add(g);
        }

    }

    public GameObject GetPooledObject() 
    {
        poolIndex %= poolCount;
        GameObject p = pooledObjects[poolIndex++];
        return p;
    }

}
