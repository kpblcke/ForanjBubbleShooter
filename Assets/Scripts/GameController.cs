using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BallLoader _ballLoader;

    public void BallHit()
    {
        _ballLoader.LoadNewBall();
    }

}
