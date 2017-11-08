using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventsManager : MonoBehaviour
{

    public static int DeadEnemies { get; set; }
    public static int AmountForEvent { get; set; }
    public ChestManager ChestManager;

    public void UpdateEvent()
    {
        if (DeadEnemies >= AmountForEvent)
        {
            SpawnEvent();
        }
    }

    public void SpawnEvent()
    {
        ChestManager.Spawn();
    }
}
