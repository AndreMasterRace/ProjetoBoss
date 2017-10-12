using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour {

	public int DamageCalc()
    {
        int damage = Random.Range(4, 8);

        return damage;
    }
}
