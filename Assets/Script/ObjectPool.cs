using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[Serializable]
public class PoolInfo
{
    public GameObject objectToPool;
    public int poolCount = 100;
    public int currentIndex = 0;
}


public interface IPoolable
{
    void Reset();
}

public class ObjectPool : MonoBehaviour
{
    [SerializeField] List<PoolInfo> poolsInfo = new();
    private Dictionary<GameObject, List<GameObject>> pools = new();

    private Dictionary<GameObject, PoolInfo> poolIndex = new();

    private static ObjectPool instance; //SINGLETON

    public static ObjectPool GetInstance() => instance; //R�F�RENCE AU SCRIPT FACILE

    private void Awake()
    {
        instance = this; //ESSENTIEL
    }

    //CR�ATION DE CHAQUE OBJETS DANS LA LIST DE POOL AU LANCEMENT DU JEU
    void Start()
    {
        foreach (PoolInfo info in poolsInfo)
        {
            List<GameObject> poolList = new();
            for (int i = 0; i < info.poolCount; i++)
            {
                GameObject g = Instantiate(info.objectToPool, Vector3.zero, Quaternion.identity, transform); //Le dernier param�tre (transfrom) sert a d�finir ObjectPool comme parent de l'object cr�er
                g.SetActive(false);
                poolList.Add(g);
            }
            pools[info.objectToPool] = poolList;
            poolIndex[info.objectToPool] = info;
        }

    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (poolIndex.TryGetValue(prefab, out PoolInfo poolInfo))
        {
            List<GameObject> poolList = pools[prefab]; //Liste temporaire
            int start = poolInfo.currentIndex;  //R�cup�re l'index de la liste demand�

            for (int i = 0; i < poolList.Count; i++)
            {
                int index = (start + i) % poolList.Count; //Permet de trouver directment la donn�e voulue, sans parcourir le pool complet. 
                GameObject g = poolList[index]; //R�cup�re l'objet indiqu� par l'index. Ainsi, plus de perfomance, moins de temps pour chercher l'objet (surtout dans un grand pool d'objet)

                if (!g.activeInHierarchy) //Si l'objet n'est pas actif, il est valide, il peut se faire return
                {
                    poolInfo.currentIndex = (index + 1) % poolList.Count; //MET A JOURS L'INDEX POUR LE PROCHAIN APPEL
                    return g;
                }
            }

            //J'AIMERAIS FAIRE QUELQUE CHOSE DE MOINS COMPLEXE COMME CECI � LA PLACE (NE FONCTIONNE PAS)
            //int index = (start) % poolList.Count;
            //GameObject g = poolList[index++];
            //return g;

        }
        else //FAIL SAFE
        {
            Debug.LogError("PREFAB NOT SET");
            return null;
        }
        return null; //SI TOUS EST DEJA ACTIFS
        //� FAIRE : RENDRE LE POOL DYNAMIQUE, SI TOUS LES OBJETS SONT ACTIF, CR�ER PLUS D'INSTANCES

        /*PREMIERE M�THODE APPRISE EN CLASSE
        poolIndex %= poolCount;
        GameObject p = pooledObjects[poolIndex++];
        return p;
        */
    }

}
