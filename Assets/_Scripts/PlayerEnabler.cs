using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnabler : MonoBehaviour {

    public static bool IsEnabled { get; set; }

    public void Disable()
    {
        GetComponent<PlayerController2>().enabled = false;
    }
    public void Enable()
    {
        GetComponent<PlayerController2>().enabled = true;
    }

    private void Update()
    {
        if(IsEnabled)
        {
            if(!GetComponent<PlayerController2>().enabled)
            {
                Enable();
            }
        }
        else
        {
            if (GetComponent<PlayerController2>().enabled)
            {
                Disable();
            }
        }
    }
}
