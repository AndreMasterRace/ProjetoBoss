using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnabler : MonoBehaviour {

    public static bool IsEnabled { get; set; }
    public static PlayerController2 PlayerController2 { get; set; }

    public static void Disable()
    {
        PlayerController2.enabled = false;

    }
    public static void Enable()
    {
        PlayerController2.enabled = true;
    }

    //private void Update()
    //{
    //    if(IsEnabled)
    //    {
    //        if(!GetComponent<PlayerController2>().enabled)
    //        {
    //            Enable();
    //        }
    //    }
    //    else
    //    {
    //        if (GetComponent<PlayerController2>().enabled)
    //        {
    //            Disable();
    //        }
    //    }
    //}
}
