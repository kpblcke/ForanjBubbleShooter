using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField]
    private int maxSteps = 10;

    public List<TrajectoryPoint> GetTrajectory(Vector2 ballPosition, Vector2 force)
    {
        CustomPhysic customPhysic = CustomPhysic.getInstance();
        List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
        
        TrajectoryPoint currentTrajectoryPoint = new TrajectoryPoint(ballPosition, force);
        trajectoryPoints.Add(currentTrajectoryPoint);
        
        for (int i = 0; i < maxSteps; i++)
        {
            currentTrajectoryPoint = customPhysic.GetNextTrajectoryPoint(currentTrajectoryPoint.Point, currentTrajectoryPoint.Velocity);
            trajectoryPoints.Add(currentTrajectoryPoint);
        }

        return trajectoryPoints;
    }

    public void DrawTrajectory(LineRenderer lineRenderer, List<TrajectoryPoint> points, float angleSpread)
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.widthMultiplier = 0.5f;
        
        for (int i = 0; i < points.Count; i++)
        {
            TrajectoryPoint point = points[i];
            lineRenderer.SetPosition(i, point.Point);
        }

        if (angleSpread > 0 && points.Count > 0)
        {
            Vector2 supVector = Quaternion.Euler(0, 0, angleSpread) *
                                (Vector2.up * Vector2.Distance(points[0].Point, points[1].Point));
            
            lineRenderer.widthMultiplier = 1f;
            lineRenderer.widthCurve = AnimationCurve.Linear(0f, lineRenderer.widthMultiplier * 0f, 
                1f, lineRenderer.widthMultiplier * Mathf.Abs(supVector.x) / CustomPhysic.getInstance().Timestep);
        }
        else
        {
            lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, lineRenderer.widthMultiplier * .5f);
        }
    }
}
