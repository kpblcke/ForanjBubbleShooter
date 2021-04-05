using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class FlyBall : Ball
{
    private bool dropped = false;
    private bool fullForce = false;

    private bool alreadyConnected = false;
    
    private CustomPhysic _customPhysic;

    private Vector2 ballVelocity;

    private void Awake()
    {
        _customPhysic = CustomPhysic.getInstance();
    }

    public void UnPathFly(Vector2 velocity)
    {
        StartCoroutine(Move(velocity));
    }

    public void SetFullForce(bool isFull)
    {
        fullForce = isFull;
    }

    public void Dropped()
    {
        dropped = true;
        gameObject.SetActive(false);
    }

    IEnumerator Move(Vector2 velocity)
    {
        float timestep = _customPhysic.Timestep;
        ballVelocity = velocity;
        while (!dropped)
        {
            TrajectoryPoint nextPoint = _customPhysic.GetNextTrajectoryPoint(transform.position, ballVelocity);
            transform.position = nextPoint.Point;
            ballVelocity = nextPoint.Velocity;
            
            yield return new WaitForSeconds(timestep);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BallConnected ballConnected = other.gameObject.GetComponent<BallConnected>();
        if (ballConnected && !alreadyConnected)
        {
            alreadyConnected = true;
            if (fullForce)
            {
                FindObjectOfType<BallGrid>().SmackBall(ballConnected, Type);
            }
            else
            {
                FindObjectOfType<BallGrid>().ConnectBall(ballConnected, transform.position, Type);
            }
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
