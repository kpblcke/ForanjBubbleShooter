using System;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private Text scoreText;

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddToScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

}
