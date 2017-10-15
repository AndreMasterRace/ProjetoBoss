using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    public static Text GameOverText;

	// Use this for initialization
	void Start () {
        GameOverText = GetComponent<Text>();
    }
	
	//public void GameOverS()
 //   {
 //       GameManager.GameOverScreen(GameOverText);
 //   }
}
