﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = System.Random;

public class EnemyMinionBehaviour : MonoBehaviour
{
    private int damage;
    private float _damageAggregate;
    public int Health;
    private int _maxHealth;
    private Animator _animator;
    public Text DamageText;
    [HideInInspector]
    //public bool IsDead;
    public LevelEventsManager LevelEventsManager;
   

    void Start()
    {
        GetComponent<NPCStats>().IsDead = false;
        _damageAggregate = 0;
        _maxHealth = Health;
        _animator = GetComponent<Animator>();

        
    }

    private void Update()
    {
        //  Transform = transform;
    }
    public IEnumerator ShowDamage()
    {
        DamageText.text = damage.ToString();

        yield return new WaitForSeconds(0.05f);
        _damageAggregate += damage;

        DamageText.text = "";

        yield return null;
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
        LevelEventsManager.DeadEnemies++;
        print(LevelEventsManager.DeadEnemies);
        LevelEventsManager.UpdateEvent();

        GetComponent<NPCStats>().IsDead = true;
        transform.position = new Vector3(40, 24, 230);
    }

    ///LEVAR DANO
    /// 
    /// 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon" && !GetComponent<NPCStats>().IsDead)
        {
            damage = other.GetComponent<WeaponBehaviour>().DamageCalc();
            Health -= damage;
            StartCoroutine(ShowDamage());
            if (Health <= 0)
            {
                Die();
            }
        }
    }
    
}
