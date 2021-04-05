using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private BallLoader _ballLoader;
    [SerializeField] private Slingshot _slingshot;

    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
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
        winCanvas.SetActive(true);
        _slingshot.Unload();
    }

    public void Defeat()
    {
        if (!winCanvas.activeInHierarchy)
        {
            loseCanvas.SetActive(true);
            _slingshot.Unload();
        }
    }

}
