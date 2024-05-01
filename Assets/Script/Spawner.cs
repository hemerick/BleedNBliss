using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 2f, 3f);

    }

    public void SpawnEnemy()
    { 
        Instantiate(enemyToSpawn, new Vector3(3f, 3f, 0), Quaternion.identity);
    }
}
