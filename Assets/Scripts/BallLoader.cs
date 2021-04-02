using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BallLoader : MonoBehaviour
{
    [SerializeField] 
    private FlyBall ballPref;
    [SerializeField] 
    private Ball nextBall;
    
    [SerializeField]
    private Text ballLeftText;
    [SerializeField] 
    private int ballsCount;

    [SerializeField] 
    private List<BallType> loadBallTypes;

    private void Start()
    {
        nextBall.ChangeType(loadBallTypes[Random.Range(0, loadBallTypes.Count)]);
    }

    public bool IsBallsLeft()
    {
        return ballsCount > 0;
    }

    private void UpdateLeftBalls()
    {
        ballLeftText.text = ballsCount.ToString();
        if (IsBallsLeft())
        {
            nextBall.ChangeType(loadBallTypes[Random.Range(0, loadBallTypes.Count)]);
        }
        else
        {
            nextBall.gameObject.SetActive(false);
        }
    }

    public FlyBall GetNewBall()
    {
        FlyBall newBall = Instantiate(ballPref, transform.position, Quaternion.identity, transform.parent) as FlyBall;
        newBall.ChangeType(nextBall.Type);
        ballsCount--;
        UpdateLeftBalls();

        return newBall;
    }
}
