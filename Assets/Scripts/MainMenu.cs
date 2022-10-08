using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    bool m_StartCalled = false;
    public GameObject m_StartScreen;
    public Slider m_Slider;
    float m_TimeSinceStart;

    void DoStartGame()
    {
        m_StartCalled = true;
        m_StartScreen.SetActive(true);
    }

    void DoQuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if(!m_StartCalled)
            return;

        m_TimeSinceStart += Time.deltaTime;

        m_Slider.value = m_TimeSinceStart / 6;

        if (m_TimeSinceStart >= 6)
            SceneManager.LoadSceneAsync(1);
    }
}
