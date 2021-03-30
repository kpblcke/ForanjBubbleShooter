using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField]
    private int maxSteps = 10;
    [SerializeField]
    private float stepSize = 1f;

    [SerializeField] 
    private GameObject trajectoryObj;

    [SerializeField]
    private int layerMask;
    private void Start()
    {
        layerMask = LayerMask.GetMask("Walls");
    }

    public List<TrajectoryPoint> GetTrajectory(Vector2 ballPosition, Vector2 force)
    {
        List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
        //Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - ball.position).normalized;
        
        TrajectoryPoint currentTrajectoryPoint = new TrajectoryPoint(ballPosition, force * stepSize); 
        for (int i = 0; i < maxSteps; i++)
        {
            currentTrajectoryPoint = GetNextTrajectoryPoint(currentTrajectoryPoint.Point, currentTrajectoryPoint.Velocity);
            trajectoryPoints.Add(currentTrajectoryPoint);
        }

        return trajectoryPoints;
    }

    private TrajectoryPoint GetNextTrajectoryPoint(Vector2 currentPoint, Vector2 currentVelocity)
    {
        Debug.DrawRay(currentPoint, currentVelocity, Color.red, 10.0f);
        RaycastHit2D hit = Physics2D.Raycast(currentPoint, currentVelocity, currentVelocity.magnitude, layerMask);
            
        if (hit.collider != null)
        {
            Vector2 reflectDirection = Vector2.Reflect((hit.point - currentPoint).normalized, hit.normal);
            // Draws a line from the normal of the object that you clicked
            Debug.DrawRay(hit.point, reflectDirection, Color.green, 10.0f);
            //Debug.DrawRay(hit.point, hit.normal, Color.yellow, 10.0f);
            
            return new TrajectoryPoint(hit.point + reflectDirection * 0.000001f, ApplyGravity(reflectDirection * currentVelocity.magnitude));
        }
        else
        {
            return new TrajectoryPoint(currentPoint + currentVelocity, ApplyGravity(currentVelocity));
        }
    }

    public void DrawTrajectory(Transform atObj, List<TrajectoryPoint> points)
    {
        foreach (Transform child in atObj) {
            Destroy(child.gameObject);
        }
        
        foreach (var point in points)
        {
            Instantiate(trajectoryObj, point.Point, Quaternion.identity, atObj);
        }
    }

    private Vector2 ApplyGravity(Vector2 force)
    {
        return new Vector2(force.x, force.y - 0.1f);
    }
}
