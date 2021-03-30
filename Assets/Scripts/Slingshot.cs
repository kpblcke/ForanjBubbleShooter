using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField]
    private Pathway _pathway;
    [SerializeField]
    private LoadedBall ball;

    [SerializeField] 
    private float maxForce = 3f;

    private bool aiming = false;

    [SerializeField] 
    private Transform trajectoryShow;
    void Update()
    {
        
    }

    private Vector2 CalculateForce()
    {
        Vector3 force = (transform.position - ball.transform.position);
        return force * Mathf.Min(force.magnitude, maxForce);
    }

    public void StartAimBall()
    {
        aiming = true;
        StartCoroutine(DrawAimTrajectory());
    }

    public void Fire()
    {
        StopAimBall();
    }
    
    public void StopAimBall()
    {
        aiming = false;
    }

    IEnumerator DrawAimTrajectory()
    {
        while (aiming)
        {
            _pathway.DrawTrajectory(trajectoryShow, _pathway.GetTrajectory(ball.transform.position, CalculateForce()));
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    IEnumerator MoveBallByTrajectory(LoadedBall ball, List<TrajectoryPoint> trajectory)
    {
        yield break;
    }
}
