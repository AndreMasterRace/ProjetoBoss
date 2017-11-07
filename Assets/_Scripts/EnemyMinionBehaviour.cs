﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMinionBehaviour : MonoBehaviour
{
    private int damage;
    private float _damageAggregate;
    public int Health;
    private int _maxHealth;
    private Animator _animator;
    public Text DamageText;
    public ChestController Chest;
    [HideInInspector]
    public bool IsDead;


    // Use this for initialization
    void Start()
    {
        IsDead = false;
        _damageAggregate = 0;
        _maxHealth = Health;
        _animator = GetComponent<Animator>();
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
        IsDead = true;
        Chest.Appear();
    }

    //LEVAR DANO
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
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
