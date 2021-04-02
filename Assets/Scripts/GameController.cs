using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallLoader _ballLoader;
    [SerializeField] private Slingshot _slingshot;

    private void Start()
    {
        BallHit();
    }

    public void BallHit()
    {
        if (_ballLoader.IsBallsLeft())
        {
            _slingshot.LoadNewBall(_ballLoader.GetNewBall());
        }
        else
        {
            Debug.Log("GAMEOVER");
        }
    }

}
