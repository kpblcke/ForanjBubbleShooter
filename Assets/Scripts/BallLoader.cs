using UnityEngine;


public class BallLoader : MonoBehaviour
{
    [SerializeField] 
    private LoadedBall ball;
    [SerializeField] 
    private Slingshot _slingshot;

    public void LoadNewBall()
    {
        _slingshot.LoadNewBall(ball);
    }
}
