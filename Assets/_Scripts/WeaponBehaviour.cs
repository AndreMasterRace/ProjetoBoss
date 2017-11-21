using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour {

    //private BoxCollider _boxCollider;
    private CapsuleCollider _capCollider;
    public bool isAttacking;

    private void Start()
    {
        isAttacking = false;
        _capCollider = GetComponent<CapsuleCollider>();
    }

    public int DamageCalc()
    {
        int damage = Random.Range(4, 8);

        return damage;
    }

    public void EnableWeaponCollider()
    {
        isAttacking = true;
        //_boxCollider.enabled = true; 
    }
    public void DisableWeaponCollider()
    {
        isAttacking = false;
       // _boxCollider.enabled = false;
    }
    //private void Update()
    //{
    //    transform.position = PlayerController2.HandTransform.position;
    //    transform.rotation = PlayerController2.HandTransform.rotation;
    //}

}
