using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour {

    private CapsuleCollider _capCollider;

    private void Start()
    {
        _capCollider = GetComponent<CapsuleCollider>();
        DisableWeaponCollider();
    }

    public int DamageCalc()
    {
        int damage = Random.Range(4, 8);

        return damage;
    }

    public void EnableWeaponCollider()
    {
        _capCollider.enabled = true;
    }
    public void DisableWeaponCollider()
    {
        _capCollider.enabled = false;
    }

}
