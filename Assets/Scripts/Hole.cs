using System;
using DefaultNamespace;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        FlyBall flyball = other.GetComponent<FlyBall>();
        if (flyball)
        {
            flyball.Dropped();
            FindObjectOfType<GameController>().LoadBall();
        }
        
        BallConnected ballConnected = other.GetComponent<BallConnected>();
        if (ballConnected)
        {
            ballConnected.PopBall();
            FindObjectOfType<GameController>().LoadBall();
        }
    }
}
