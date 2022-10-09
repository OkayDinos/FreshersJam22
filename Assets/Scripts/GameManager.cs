using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Playing, Paused, Ended }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    float score;

    public GameState currentGameState = GameState.Playing;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance)
            Destroy(this);
        else
            GameManager.instance = this;

        StartGame();
    }

    void StartGame()
    {
        score = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void DoPauseGame()
    {
        currentGameState = GameState.Paused;
        PauseMenu.instance.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        // It's over
    }
}
