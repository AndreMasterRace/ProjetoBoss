using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static List<GameObject> LifePointList { get; set; }
    public static GameObject[] LifePointArray { get; set; }
    public GameObject LifeBarPanel;
    public Canvas Canvas;

    private void Awake()
    {
        PlayerEnabler.IsEnabled = true;
        LevelEventsManager.DeadEnemies = 0;
        LevelEventsManager.AmountForEvent = 2;

    }
    private void Start()
    {

        LifePointList = new List<GameObject>();

        LifePointArray = new GameObject[20];
        
        for (int i = 0; i < 20; i++)
        {

            LifePointArray[i] = Resources.Load<GameObject>("Prefabs/LifePoint");

            LifePointList.Add(GameObject.Instantiate(LifePointArray[i], LifeBarPanel.transform));
        }

    }

    //TIRAR PONTOS DE VIDA NA BARRA, AO PLAYER
    public static void LifeBarDecrease(int damage, int totalDamageTaken)
    {
        print(PlayerController.MaxHealth);
        for (int i = PlayerController.MaxHealth-1 - totalDamageTaken; i > PlayerController.MaxHealth-1-totalDamageTaken - damage && i>=0; i--)
        {
            LifePointList[i].GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    //FAZER GAME OVER
    public static void GameOverScreen()
    {
        GameOver.GameOverText.text = "You Died";
    }


   
}
