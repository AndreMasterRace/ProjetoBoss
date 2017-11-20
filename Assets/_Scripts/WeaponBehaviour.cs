using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour {

    private BoxCollider _boxCollider;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public int DamageCalc()
    {
        int damage = Random.Range(4, 8);

        return damage;
    }

    public void EnableWeaponCollider()
    {
        _boxCollider.enabled = true; 
    }
    public void DisableWeaponCollider()
    {
        _boxCollider.enabled = false;
    }
    //private void Update()
    //{
    //    transform.position = PlayerController2.HandTransform.position;
    //    transform.rotation = PlayerController2.HandTransform.rotation;
    //}

}
