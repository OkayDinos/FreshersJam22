using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public Button _resumeButon;

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

    private void OnEnable()
    {
        _resumeButon.Select();
    }
    void DoResumeGame()
    {
        GameManager.instance.DoPauseGame();
    }

    void DoQuitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
