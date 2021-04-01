using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slingshot : MonoBehaviour
{
    [SerializeField]
    private Pathway _pathway;
    [SerializeField]
    private LoadedBall ball;

    [SerializeField]
    private FlyBall _flyBall;

    [SerializeField] 
    private float forceMultiply = 4f;
    [SerializeField] 
    private float maxTension = 1f;
    [SerializeField] 
    private float tensionAngleSpread = 30f;
    
    private bool aiming = false;

    [SerializeField] 
    private LineRenderer trajectoryShow;
    [SerializeField]
    private LineRenderer rubber;
    private List<TrajectoryPoint> _points;
    [SerializeField]
    private Transform rubberCenter;

    public float MAXTension => maxTension;

    private void Start()
    {
        rubber = GetComponent<LineRenderer>();
        
        rubber.SetPosition(0, transform.position + Vector3.left * maxTension);
        rubber.SetPosition(1, ball.transform.position);
        rubber.SetPosition(2, transform.position - Vector3.left * maxTension);
    }

    private Tuple<Vector2, bool> CalculateForce()
    {
        Vector2 force = transform.position - ball.transform.position;
        float tensionStrength = force.magnitude / maxTension;
        return new Tuple<Vector2, bool> (force.normalized * tensionStrength * forceMultiply, Mathf.Approximately(force.magnitude, maxTension));
    }

    public void StartAimBall()
    {
        aiming = true;
        trajectoryShow.gameObject.SetActive(true);
        StartCoroutine(DrawAimTrajectory());
    }

    public void Fire()
    {
        StopAimBall();
        ball.gameObject.SetActive(false);
        trajectoryShow.gameObject.SetActive(false);
        FlyBall fly = Instantiate(_flyBall, ball.transform.position, Quaternion.identity);
        Tuple<Vector2, bool> calculatedForce = CalculateForce();
        if (calculatedForce.Item2)
        {
            Vector2 randomizedVector = Quaternion.Euler(0, 0, -tensionAngleSpread) *
                                    calculatedForce.Item1;
            fly.UnPathFly(randomizedVector);
        }
        else
        {
            fly.UnPathFly(calculatedForce.Item1);
        }

        StartCoroutine(ElasticShot(fly.transform));
    }
    
    public void StopAimBall()
    {
        aiming = false;
    }

    private void DrawElastic(Vector3 atPosition)
    {
        rubber.SetPosition(1, atPosition);
    }

    public void LoadNewBall(LoadedBall newBall)
    {
        ball.transform.position = transform.position;
        ball.gameObject.SetActive(true);
        rubber.SetPosition(1, ball.transform.position);
    }
    
    IEnumerator DrawAimTrajectory()
    {
        while (aiming)
        {
            Tuple<Vector2, bool> forceAndType = CalculateForce();
            _points = _pathway.GetTrajectory(ball.transform.position, forceAndType.Item1);
            _pathway.DrawTrajectory(trajectoryShow, _points, forceAndType.Item2 ? tensionAngleSpread : 0f);
            DrawElastic(ball.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    IEnumerator ElasticShot(Transform obj)
    {
        while (Vector2.Distance(obj.position, transform.position) < maxTension)
        {
            rubber.SetPosition(1, obj.position);
            yield return new WaitForEndOfFrame();
        }
        
        rubberCenter.transform.position = obj.position;

        while (!aiming)
        {
            rubber.SetPosition(1, rubberCenter.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
}
