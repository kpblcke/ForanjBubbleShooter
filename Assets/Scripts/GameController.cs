using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallLoader _ballLoader;
    [SerializeField] private Slingshot _slingshot;

    private void Start()
    {
        LoadBall();
    }

    public void LoadBall()
    {
        if (_slingshot.IsLoaded())
        {
            return;
        }
        if (_ballLoader.IsBallsLeft())
        {
            _slingshot.LoadNewBall(_ballLoader.GetNewBall());
        }
        else
        {
            Defeat();
        }
    }

    public void Win()
    {
        Debug.Log("Win");
    }

    public void Defeat()
    {
        Debug.Log("GAMEOVER");
    }

}
