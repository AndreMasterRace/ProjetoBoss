using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnabler : MonoBehaviour {


    public static bool IsEnabled { get; set; }
    public static BossBehaviour2 BossBehaviour2 { get; set; }

    public static void Disable()
    {
        IsEnabled = false;
        BossBehaviour2.enabled = false;
    }
    public static void Enable()
    {
        IsEnabled = true;
        BossBehaviour2.enabled = true;
    }
}
