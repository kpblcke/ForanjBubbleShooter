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
    private FlyBall _flyBall;
    
    [SerializeField] 
    private float forceMultiply = 4f;
    [SerializeField] 
    private float maxTension = 1f;
    [SerializeField] 
    private float tensionAngleSpread = 30f;
    
    private bool aiming = false;
    private bool loaded = false;

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

    /// <summary>
    /// Запустить шар
    /// </summary>
    public void Fire()
    {
        StopAimBall();
        trajectoryShow.gameObject.SetActive(false);
        _flyBall.transform.position = ball.transform.position;
        _flyBall.gameObject.SetActive(true);
        ball.gameObject.SetActive(false);

        Tuple<Vector2, bool> calculatedForce = CalculateForce();
        _flyBall.SetFullForce(calculatedForce.Item2);
        if (calculatedForce.Item2)
        {
            Vector2 randomizedVector = Quaternion.Euler(0, 0, Random.Range(-tensionAngleSpread, tensionAngleSpread)) *
                                    calculatedForce.Item1;
            _flyBall.UnPathFly(randomizedVector);
        }
        else
        {
            _flyBall.UnPathFly(calculatedForce.Item1);
        }

        StartCoroutine(ElasticShot(_flyBall.transform));
        loaded = false;
    }
    
    public void StopAimBall()
    {
        aiming = false;
    }
    
    public void Unload()
    {
        ball.StopInteraction();
    }

    private void DrawElastic(Vector3 atPosition)
    {
        rubber.SetPosition(1, atPosition);
    }

    /// <summary>
    /// загрузить новый шар для выстрела
    /// </summary>
    public void LoadNewBall(FlyBall newBall)
    {
        _flyBall = newBall;
        _flyBall.transform.position = transform.position;
        _flyBall.gameObject.SetActive(false);
        
        ball.ChangeType(newBall.Type);
        ball.transform.position = transform.position;
        ball.gameObject.SetActive(true);
        rubber.SetPosition(1, ball.transform.position);
        loaded = true;
    }

    public bool IsLoaded()
    {
        return loaded;
    }
    
    IEnumerator DrawAimTrajectory()
    {
        while (aiming)
        {
            Tuple<Vector2, bool> forceAndType = CalculateForce();
            _points = _pathway.GetTrajectory(ball.transform.position, forceAndType.Item1);
            _pathway.DrawTrajectory(trajectoryShow, _points, forceAndType.Item2 ? tensionAngleSpread : 0f);
            DrawElastic(ball.transform.position);
            yield return new WaitForEndOfFrame();
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

        while (!ball.isActiveAndEnabled)
        {
            rubber.SetPosition(1, rubberCenter.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }
}
