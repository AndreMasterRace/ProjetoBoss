using System.Collections;
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


    // Use this for initialization
    void Start()
    {
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

    //LEVAR DANO
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            //print("attack");
            damage = other.GetComponent<WeaponBehaviour>().DamageCalc();
            Health -= damage;
            StartCoroutine(ShowDamage());
            if (Health <= 0)
            {
                _animator.SetBool("isDead", true);
                Chest.Appear();
            }

        }
    }

}
