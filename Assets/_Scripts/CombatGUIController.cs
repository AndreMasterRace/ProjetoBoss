using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGUIController : MonoBehaviour {

    private void Update()
    {
        transform.LookAt(PlayerController2.Transform);
    }
}
