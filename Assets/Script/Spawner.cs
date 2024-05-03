using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;

    private void Start()
    {
        //Coroutine est une méthode qui peut inclure des délais de temps
        StartCoroutine(SpawnEnemy());
    }


    public IEnumerator SpawnEnemy()
    {
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {

                Instantiate(enemyToSpawn, new Vector3(3f, 3f, 0), Quaternion.identity);
            }
                yield return new WaitForSeconds(3);
        }
    }
}
