using System;
using UnityEngine;

public class TrajectoryPoint
{
    private Vector2 point;
    private Vector2 velocity;

    public Vector2 Point => point;

    public Vector2 Velocity => velocity;

    public TrajectoryPoint(Vector2 point, Vector2 velocity)
    {
        this.point = point;
        this.velocity = velocity;
    }
    
    public TrajectoryPoint(Tuple<Vector2, Vector2> tuple)
    {
        point = tuple.Item1;
        velocity = tuple.Item2;
    }

}