using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;

public class EndGame : MonoBehaviour
{
    int _finalScore;
    public GameObject _scoreTextGO;
    public TMP_InputField _usernameInpF;
    public Button _submitButton;

    void Start()
    {
        //* Get Score
        _finalScore = 0;


        _scoreTextGO.GetComponent<TextMeshProUGUI>().SetText($"You Scored:\n{_finalScore}");
    }

    void NameChanged()
    {
        if (_usernameInpF.text.Length < 3)
        {
            _submitButton.interactable = false;
        }
        else
        {
            _submitButton.interactable = true;
        }


    }

    public void SubmitScore()
    {
        if (_usernameInpF.text.Length < 3)
            return;

        _submitButton.interactable = false;
        _usernameInpF.interactable = false;
        _submitButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Submitting...");
        StartCoroutine(DoPostScores(_usernameInpF.text, _finalScore));
    }

    public void DoExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private const string scoreURL = "https://karatekevin.pdox.uk/setscore.php";

    IEnumerator DoPostScores(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post(scoreURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                _submitButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Error");
            }
            else
            {
                _submitButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Submited");
            }
        }
    }
}