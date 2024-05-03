using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //[SerializeField] GameObject[] listOfEnemy;
    [SerializeField] float spawnRadius = 5f;
    [SerializeField] private GameObject enemyToSpawn;
    public int spawnAmount = 3;

    private static Spawner instance;
    
    public static Spawner GetInstance() => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Coroutine est une méthode qui peut inclure des délais de temps
        StartCoroutine(SpawnEnemy());
    }


    public IEnumerator SpawnEnemy()
    {
        while (true)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject enemy = ObjectPool.GetInstance().GetPooledObject(enemyToSpawn);
                Vector3 spawnPosition = RandomPositionAroundPlayer();
                enemy.transform.position = spawnPosition;

                enemy.GetComponent<IPoolable>().Reset();
                enemy.SetActive(true);
            }
                yield return new WaitForSeconds(15);
            spawnAmount *= 2;
        }
    }

    private Vector3 RandomPositionAroundPlayer() 
    {
        Vector3 playerPosition = Player.GetInstance().transform.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
        return new Vector3(playerPosition.x + randomDirection.x, playerPosition.z + randomDirection.y);
    }
}
