using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Playing, Paused, Ended }
public enum PointsType { NormalAttack, FourForThree, Uppercut, EatSausageRoll, WrapperPickup };
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI MainScoreText, scoreAdditionText, scoreAdditionTextEcho;
    public RectTransform scoreAdditionTransform, scoreAdditionTransformEcho;

    public int score;

    public GameState currentGameState = GameState.Playing;

    public GameObject endGamePanel;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance)
        {
            Destroy(this.gameObject);
            return;
        }

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

    public void DoPauseGame()


    {

        Debug.Log("rrerererer");
        switch (currentGameState)
        {
            case GameState.Playing:
                currentGameState = GameState.Paused;
                PauseMenu.instance.gameObject.SetActive(true);
                break;
            case GameState.Paused:
                currentGameState = GameState.Playing;
                PauseMenu.instance.gameObject.SetActive(false);
                break;
            case GameState.Ended:
                break;
            default:
                break;
        }

    }

    /*public void AddScore(int scoreToAdd,string boomText) 
    {
        score += scoreToAdd;
        MainScoreText.SetText(score.ToString("0000"));
        scoreAdditionText.color = new Color(0, 255, 0, 0);
        scoreAdditionText.SetText(boomText);
        scoreAdditionTransform.localScale = Mathf.Lerp(new Vector3(1,1,1), new Vector3(2, 2, 2));

    }*/
    int lastTextRandomiser = 0, textSelector = 0;
    public async void AddScore(PointsType pointsType, float pointsMultiplier = 1)
    {
        float time = 0.5f;

        float timer = 0;

        string boomText = "";
        Color boomColour;
        int scoreToAdd = 0;

        //Colour & Text Selector
        switch (pointsType)
        {
            case PointsType.NormalAttack:
                do { textSelector = Random.Range(0, 5); } while (textSelector == lastTextRandomiser);
                boomColour = new Color32(0, 255, 0, 255);
                scoreToAdd = 1000;
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
                scoreToAdd = 1500;
                boomColour = new Color32(255, 200, 0, 255);
                break;
            case PointsType.Uppercut:
                boomText = "Uppercut!";
                scoreToAdd = 2000;
                boomColour = new Color32(255, 200, 0, 255);
                break;
            case PointsType.EatSausageRoll:
                do { textSelector = Random.Range(0, 5); } while (textSelector == lastTextRandomiser);
                boomColour = new Color32(242, 157, 15, 255);
                scoreToAdd = Mathf.RoundToInt(250 * pointsMultiplier);
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
            case PointsType.WrapperPickup:
                do { textSelector = Random.Range(0, 5); } while (textSelector == lastTextRandomiser);
                boomColour = new Color32(220, 220, 220, 255);
                scoreToAdd = 50;
                switch (textSelector)
                {
                    case 0:
                        boomText = "Good Citizen";
                        break;
                    case 1:
                        boomText = "Litter Pecker!";
                        break;
                    case 2:
                        boomText = "City Cleanup";
                        break;
                    case 3:
                        boomText = "Trash Trashed!";
                        break;
                    case 4:
                        boomText = "+ Used Sausage Wrapper";
                        break;
                }
                break;
            default:
                boomText = "";
                boomColour = new Color32(0, 0, 0, 0);
                break;
        }



        score += scoreToAdd;
        MainScoreText.SetText(score.ToString("0000"));

        boomText += scoreToAdd >= 0 ? $"\n<size=60%>+{scoreToAdd.ToString()}" : $"\n<size=60%>-{scoreToAdd.ToString()}";
        scoreAdditionText.color = boomColour;
        Color boomColourTransparent = new Color(boomColour.r, boomColour.g, boomColour.b, 0);
        Color boomColourEchoStarter = new Color(boomColour.r, boomColour.g, boomColour.b, 0.5f);
        scoreAdditionTextEcho.color = boomColourTransparent;
        scoreAdditionText.SetText(boomText);
        scoreAdditionTextEcho.SetText(boomText);
        scoreAdditionTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-21, 20));
        scoreAdditionTransformEcho.rotation = Quaternion.Euler(0, 0, scoreAdditionTransform.rotation.eulerAngles.z + Random.Range(-1, 2));

        while (timer < time)
        {
            timer += Time.deltaTime;

            scoreAdditionTransform.localScale = Vector3.Lerp(new Vector3(2, 2, 2), new Vector3(1, 1, 1), (Mathf.Exp(timer) - 1) / time);
            scoreAdditionText.color = Vector4.Lerp(boomColourTransparent, boomColour, (Mathf.Exp(timer) - 1) / time);

            await System.Threading.Tasks.Task.Yield();
        }
        timer = 0;
        while (timer < time)
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

        lastTextRandomiser = textSelector;
    }

    public void GameOver()
    {
        // It's over (no its not because keven knows karate)

        if (currentGameState != GameState.Ended)
            endGamePanel.SetActive(true);

        currentGameState = GameState.Ended;


    }
}
