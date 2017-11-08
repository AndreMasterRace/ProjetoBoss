using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnController : MonoBehaviour
{

    public List<EnemyMinionBehaviour> EnemyList;
    [HideInInspector]
    public bool TheresEnemiesOnSight;

    private void Start()
    {
        TheresEnemiesOnSight = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            ///SE NAO HAVIA INIMIGOS PERTO
            if (EnemyList.ToArray().Length == 0)
            {
                print("enemy enter");
                TheresEnemiesOnSight = true;
                EnemyList.Add(other.GetComponent<EnemyMinionBehaviour>());
            }
            ///SE JA HA OUTROS INIMIGOS PERTO
            StartCoroutine(CheckIfItsCopy(other.GetComponent<EnemyMinionBehaviour>()));
            ///
        }
    }
    private IEnumerator CheckIfItsCopy(EnemyMinionBehaviour newEnemy)
    {
        int nDifferentEnemies = 0;
        yield return new WaitForFixedUpdate();
        foreach (EnemyMinionBehaviour enemy in EnemyList.ToArray())
        {
            if (enemy.GetInstanceID() != newEnemy.GetInstanceID())
            {
                nDifferentEnemies++;
            }
        }
        if(nDifferentEnemies == EnemyList.ToArray().Length)
        {
            print("enemy enter");
            TheresEnemiesOnSight = true;
            EnemyList.Add(newEnemy.GetComponent<EnemyMinionBehaviour>());
        }

        yield return null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyList.Remove(other.GetComponent<EnemyMinionBehaviour>());
            if (EnemyList.ToArray().Length == 0)
            {
                TheresEnemiesOnSight = false;
            }
        }
    }

    public EnemyMinionBehaviour CheckCloser()
    {
        ///TENHO DE COLOCAR UM INIMIGO DEFAULT
        EnemyMinionBehaviour closerEnemy = EnemyList[0];
        ///
        float distance = 1000;


        foreach (EnemyMinionBehaviour enemy in EnemyList.ToArray())
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
