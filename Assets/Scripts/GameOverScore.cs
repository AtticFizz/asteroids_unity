using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScore : MonoBehaviour
{
    public GameObject GameOverScoreGO;
    public GameObject HighScoreGO;
    public GameObject ScoreGO;
    
    public Text gameOverScore;

    void Start()
    {
        gameOverScore.enabled = false;
    }

    public void WriteScoreText()
    {
        if (HighScoreGO.GetComponent<HighScore>().highScore == true)
        {
            gameOverScore.text = "NEW HIGHSCORE!";
        }
        else if (HighScoreGO.GetComponent<HighScore>().highScore == false)
        {
            gameOverScore.text = "YOUR SCORE: " + ScoreGO.GetComponent<Score>().score.ToString();
        }

        gameOverScore.enabled = true;
    }

}
