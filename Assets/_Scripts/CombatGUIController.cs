using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatGUIController : MonoBehaviour {

    public static bool InCombat { get; set; }


    private void Update()
    {
        //if(InCombat)
        //{
            transform.LookAt(PlayerController2.Transform);
        //}
        
    }
}
