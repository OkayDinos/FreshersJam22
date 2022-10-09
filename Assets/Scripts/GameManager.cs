using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Playing, Paused, Ended }
public enum PointsType { NormalAttack, FourForThree, Uppercut, EatSausageRoll };
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI MainScoreText, scoreAdditionText, scoreAdditionTextEcho;
    public RectTransform scoreAdditionTransform, scoreAdditionTransformEcho;

    float score;

    public GameState currentGameState = GameState.Playing;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance)
            Destroy(this);
        else
            GameManager.instance = this;

        MainScoreText = GameObject.Find("TextMainScore").GetComponent<TextMeshProUGUI>();
        scoreAdditionText = GameObject.Find("ScoreAdditionText").GetComponent<TextMeshProUGUI>();
        scoreAdditionTransform = scoreAdditionText.GetComponent<RectTransform>();
        scoreAdditionTextEcho = GameObject.Find("ScoreAdditionTextEcho").GetComponent<TextMeshProUGUI>();
        scoreAdditionTransformEcho = scoreAdditionTextEcho.GetComponent<RectTransform>();

        StartGame();
    }

    void StartGame()
    {
        score = 0;
        MainScoreText.SetText(score.ToString("0000"));
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

    /*public void AddScore(int scoreToAdd,string boomText) 
    {
        score += scoreToAdd;
        MainScoreText.SetText(score.ToString("0000"));
        scoreAdditionText.color = new Color(0, 255, 0, 0);
        scoreAdditionText.SetText(boomText);
        scoreAdditionTransform.localScale = Mathf.Lerp(new Vector3(1,1,1), new Vector3(2, 2, 2));

    }*/
    public async void AddScore(int scoreToAdd, PointsType pointsType)
    {
        float time = 0.5f;

        float timer = 0;

        string boomText = "";
        Color boomColour;
        int textSelector;

        //Colour & Text Selector
        switch (pointsType)
        {
            case PointsType.NormalAttack:
                textSelector = Random.Range(0, 4);
                boomColour = new Color(0, 255, 0, 1);
                switch (textSelector)
                {
                    case 0:
                        boomText = "Granny Kevind'";
                        break;
                    case 1:
                        boomText = "Elderly Extradited";
                        break;
                    case 2:
                        boomText = "Nana No More";
                        break;
                    case 3:
                        boomText = "Grandpa Goodbye";
                        break;
                    case 4:
                        boomText = "Food Handler Finessed";
                        break;
                }

                break;
            case PointsType.FourForThree:
                boomText = "4 For 3";
                boomColour = new Color(255, 200, 0, 1);
                break;
            case PointsType.Uppercut:
                boomText = "Uppercut!";
                boomColour = new Color(255, 200, 0, 1);
                break;
            case PointsType.EatSausageRoll:
                textSelector = Random.Range(0, 4);
                boomColour = new Color(200, 120, 0, 1);
                switch (textSelector)
                {
                    case 0:
                        boomText = "I Love Sausage!";
                        break;
                    case 1:
                        boomText = "Mmm Warm Sausage";
                        break;
                    case 2:
                        boomText = "Nom Nom Nom";
                        break;
                    case 3:
                        boomText = "Glizzy Gobbled";
                        break;
                    case 4:
                        boomText = "Filled up by the Sausage!";
                        break;
                }
                break;
            default:
                boomText = "";
                boomColour = new Color(0, 0, 0, 0);
                break;
        }



        score += scoreToAdd;
        MainScoreText.SetText(score.ToString("0000"));
        boomText += $"\n<size=60%>+{scoreToAdd.ToString("0000")}";
        scoreAdditionText.color = boomColour;
        Color boomColourTransparent = new Color(boomColour.r, boomColour.g, boomColour.b, 0);
        Color boomColourEchoStarter = new Color(boomColour.r, boomColour.g, boomColour.b, 0.5f);
        scoreAdditionTextEcho.color = boomColourTransparent;
        scoreAdditionText.SetText(boomText);
        scoreAdditionTextEcho.SetText(boomText);
        scoreAdditionTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-21, 20));
        scoreAdditionTransformEcho.rotation = Quaternion.Euler(0,0, scoreAdditionTransform.rotation.eulerAngles.z + Random.Range(-1, 2));

        while (timer < time)
        {
            timer += Time.deltaTime;

            scoreAdditionTransform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), new Vector3(1, 1, 1), (Mathf.Exp(timer)-1)/time);
            scoreAdditionText.color = Vector4.Lerp(boomColourTransparent, boomColour, (Mathf.Exp(timer) - 1) / time);

            await System.Threading.Tasks.Task.Yield();
        }
        timer = 0;
        while(timer < time)
        {
            timer += Time.deltaTime;
            scoreAdditionTransformEcho.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(3, 3, 3), (Mathf.Exp(timer) - 1) / time);
            scoreAdditionTextEcho.color = Vector4.Lerp(boomColourEchoStarter, boomColourTransparent, (Mathf.Exp(timer) - 1) / (time - 0.2f));
            await System.Threading.Tasks.Task.Yield();
        }
        timer = 0;
        time = 0.2f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            scoreAdditionTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0, 0, 0), (Mathf.Exp(timer) - 1) / time);
            scoreAdditionText.color = Vector4.Lerp(boomColour, boomColourTransparent, (Mathf.Exp(timer) - 1) / time);
            await System.Threading.Tasks.Task.Yield();
        }

        //scoreAdditionTransform.localScale = new Vector3(1, 1, 1);
        //scoreAdditionText.color = new Color(0, 255, 0, 0);
    }

    public void GameOver()
    {
        // It's over
    }
}
