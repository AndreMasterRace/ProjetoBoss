﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour {

    public int Damage;

    private void OnTriggerEnter(Collider other)
    {
      if(other.tag == "Player")
        {
            //other.GetComponent<PlayerController>().TakeDamage(Damage);
            other.GetComponent<PlayerController2>().TakeDamage(Damage);
        }
    }
}
