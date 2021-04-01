using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class FlyBall : MonoBehaviour
{
    private bool dropped = false;
    private bool connected = false;
    
    private CustomPhysic _customPhysic;

    private void Awake()
    {
        
        _customPhysic = CustomPhysic.getInstance();
    }

    public void UnPathFly(Vector2 velocity)
    {
        StartCoroutine(Move(velocity));
    }

    public void HitBall()
    {
        connected = true;
    }

    public void Dropped()
    {
        dropped = true;
        gameObject.SetActive(false);
    }

    IEnumerator Move(Vector2 velocity)
    {
        float timestep = _customPhysic.Timestep;
        while (!dropped && !connected)
        {
            TrajectoryPoint nextPoint = _customPhysic.GetNextTrajectoryPoint(transform.position, velocity);
            transform.position = nextPoint.Point;
            velocity = nextPoint.Velocity;
            
            yield return new WaitForSeconds(timestep);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Ball>())
        {
            connected = true;
            FindObjectOfType<GameController>().BallHit();
        }
    }
}
