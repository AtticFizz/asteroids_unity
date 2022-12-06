using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    public GameObject ScoreGO;
    public GameObject HighScoreGO;
    public Text highScoreText;

    public bool highScore;

    void Start()
    {
        highScore = false;

        highScoreText.enabled = false;
        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void CheckHighScore()
    {
        if (PlayerPrefs.GetInt("HighScore", 0) < ScoreGO.GetComponent<Score>().score)
        {
            PlayerPrefs.SetInt("HighScore", ScoreGO.GetComponent<Score>().score);
            highScore = true;
        }
    }

    public void WriteHighScoreText()
    {
        if (!highScore)
            highScoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        if (highScore)
            highScoreText.text = "YOUR SCORE: " + ScoreGO.GetComponent<Score>().score.ToString();

        highScoreText.enabled = true;
    }
}
