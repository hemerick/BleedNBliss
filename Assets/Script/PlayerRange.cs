using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerRange : MonoBehaviour, IUpdatePlayerRange
{
    public List<GameObject> targets = new();
    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider= GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        targets.RemoveAll(target => target == null || !target.activeInHierarchy);
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            targets.Add(trigger.gameObject);
        }

    }

    //RETIRE LES ENEMY OUT OF RANGE DE LA LISTE DE TARGET
    private void OnTriggerExit2D(Collider2D trigger)
    {
        //VÉRIFIE SI LA COLLISION EST AVEC UN ENEMY
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            targets.Remove(trigger.gameObject);
        }
        else if(trigger.gameObject.CompareTag("PlayerProjectile"))
        {
            trigger.gameObject.SetActive(false);
        }
    }

    public GameObject ClosestTarget()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;


        //POUR CHAQUE ENEMY DANS LA LISTE DE TARGET, COMPARE LA DISTANCE
        foreach (GameObject target in targets)
        {
            if(!target.activeInHierarchy) { continue; }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        //RETOURNE L'ENEMY LE PLUS PROCHE DU JOUEUR
        return closestTarget;
    }

    public void UpdatePlayerRange(float range)
    {
        circleCollider.radius = range;
    }
}
