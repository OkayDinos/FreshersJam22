using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    // Start is called before the first frame update
    void Start()
    {
        if (PauseMenu.instance)
            Destroy(this);
        else
            PauseMenu.instance = this;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoResumeGame()
    {
        GameManager.instance.currentGameState = GameState.Playing;
        gameObject.SetActive(false);
    }

    void DoQuitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
