
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class Scoreboard : MonoBehaviour
{
    public Button _leaderboardButton, _mainMenuButton;
    public List<GameObject> hideObjects = new List<GameObject>();
    public string _leaderboardCommaSeperated;
    public string[] splitArray;
    public GameObject m_PanelLeaderboard;
    public TextMeshProUGUI[] rank, playername, score;
    public int scoreboardstarter = 1;

    public void ShowLeaderboard()
    {



        _leaderboardButton.interactable = false;
        _leaderboardButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Fetching...");
        StartCoroutine(DoRetrieveScores());
    }

    public void Exit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private const string highscoreURL = "http://karatekevin.pdox.uk/getscores.php";

    IEnumerator DoRetrieveScores()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(highscoreURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                _leaderboardButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Error");
            }
            else
            {
                m_PanelLeaderboard.SetActive(true);

                foreach (GameObject item in hideObjects)
                {
                    item.SetActive(false);
                }

                _mainMenuButton.Select();

                _leaderboardCommaSeperated = www.downloadHandler.text;
                SplitMe();
                yield return null;
            }
        }
    }



    void SplitMe()
    {
        splitArray = _leaderboardCommaSeperated.Split(char.Parse(";"));

        WhatTFDoICallTHis(scoreboardstarter);
    }

    void WhatTFDoICallTHis(int start)
    {
        start--;

        for (int i = 0; i < 10; i++)
        {
            rank[i].text = (start + i + 1).ToString();
            playername[i].text = splitArray[(start + i)].Split(char.Parse(","))[0];
            score[i].text = splitArray[(start + i)].Split(char.Parse(","))[1];
        }
    }

    public void nextl(int number)
    {
        int l_PreviousStarter = scoreboardstarter;

        scoreboardstarter += number;

        scoreboardstarter = Mathf.Clamp(scoreboardstarter, 1, 41);

        Debug.Log($"StartFromScore: {scoreboardstarter} | TotalScores: {splitArray.Length}");
        if (scoreboardstarter + 8 < splitArray.Length)
            WhatTFDoICallTHis(scoreboardstarter);
        else
        {
            scoreboardstarter = l_PreviousStarter;
        }
    }
}