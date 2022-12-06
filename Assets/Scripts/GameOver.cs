using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text gameOverText;
    void Start()
    {
        gameOverText.enabled = false;
    }

    public void WriteGameOverText()
    {
        gameOverText.enabled = true;
    }
}
