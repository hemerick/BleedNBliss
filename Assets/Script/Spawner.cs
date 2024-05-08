using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnInfo 
{
    public GameObject enemy;
    public float spawnChance;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<SpawnInfo> spawnInfos; //Liste des informations du monstre à faire spawn
    [SerializeField] private GameObject defaultEnemy;
    [SerializeField] private float spawnRadius = 10f;
    public int spawnAmount = 3;

    private static Spawner instance;
    
    public static Spawner GetInstance() => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        float totalPercentage = 0f;
        foreach (var info in spawnInfos)
        {
            totalPercentage += info.spawnChance;
        }

        //VÉRIFIE QUE LE TOTAL DES POURCENTAGES NE DÉPASSE PAS 100%
        if (totalPercentage > 100f)
        {
            Debug.LogWarning("TOTAL SPAWN CHANCE EXCEED 100%, SOME ENEMY WONT SPAWN | TOTALPERCENTAGE : " + totalPercentage);
        }

        //Coroutine est une méthode qui peut inclure des délais de temps
        StartCoroutine(SpawnEnemy());
    }


    public IEnumerator SpawnEnemy()
    {
        while (!Player.GetInstance().isDead)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject enemy = ObjectPool.GetInstance().GetPooledObject(SelectEnemyToSpawn());
                Vector3 spawnPosition = RandomPositionAroundPlayer();
                enemy.transform.position = spawnPosition;

                enemy.GetComponent<IPoolable>().Reset();
                enemy.SetActive(true);
            }
                yield return new WaitForSeconds(7);
            spawnAmount += (spawnAmount/10);
            spawnAmount++;
        }
    }

    private GameObject SelectEnemyToSpawn() 
    {
        float randomPoint = UnityEngine.Random.value * 100;
        float currentSum = 0f;
        foreach (var info in spawnInfos)
        {
            currentSum += info.spawnChance;
            if (currentSum >= randomPoint)
            {
                return info.enemy;
            }

        }
        return defaultEnemy; //Retourne le monstre de base si aucun monstre Spécial est choisi
    }


    private Vector3 RandomPositionAroundPlayer() 
    {
        Vector3 playerPosition = Player.GetInstance().transform.position;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        return new Vector3(playerPosition.x + randomDirection.x, playerPosition.z + randomDirection.y);
    }
}
