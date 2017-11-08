using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnController : MonoBehaviour
{

    public List<GameObject> EnemyList;

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
        if (other.tag == "Enemy")
        {
            EnemyList.Add(other.GetComponent<GameObject>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyList.Remove(other.GetComponent<GameObject>());
        }
    }

    public GameObject CheckCloser()
    {
        GameObject closerEnemy = new GameObject();
        float distance = 1000;


        foreach (GameObject enemy in EnemyList)
        {
            if (Vector3.Distance(PlayerController2.Transform.position, enemy.transform.position) < distance)
            {
                distance = Vector3.Distance(PlayerController2.Transform.position, enemy.transform.position);
                closerEnemy = enemy;
            }
        }

        return closerEnemy;
    }



}
