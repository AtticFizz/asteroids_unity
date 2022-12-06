using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public int score;

    void Start()
    {
        score = 0;
        scoreText.enabled = true;
        WriteScore();
    }

    public void WriteScore()
    {
        scoreText.text = "SCORE: " + score.ToString();
    }
}
