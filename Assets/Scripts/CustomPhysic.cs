using DefaultNamespace;
using UnityEngine;

public class CustomPhysic
{
    private static CustomPhysic instance;
    [SerializeField]
    private float GRAVITY = 9.8f;
    [SerializeField]
    private float TIMESTEP = 0.01f;
    
    int layerMask;

    public float Timestep => TIMESTEP;

    private CustomPhysic()
    {
        layerMask = LayerMask.GetMask("Walls", "Balls");
    }

    public static CustomPhysic getInstance()
    {
        if (instance == null)
        {
            instance = new CustomPhysic();
        }

        return instance;
    }
    
    public TrajectoryPoint GetNextTrajectoryPoint(Vector2 startPoint, Vector2 velocity)
    { 
        RaycastHit2D hit = Physics2D.Raycast(startPoint, velocity, velocity.magnitude * TIMESTEP, layerMask);

        if (hit.collider != null)
        {
            startPoint = hit.point;
            if (hit.collider.GetComponent<Ball>())
            {
                return new TrajectoryPoint(startPoint, velocity);
            }
            
            velocity = Vector2.Reflect(velocity, hit.normal);
            return new TrajectoryPoint(startPoint + velocity * 0.0001f, ApplyGravity(velocity, TIMESTEP));
        }

        return new TrajectoryPoint(startPoint + velocity * TIMESTEP, ApplyGravity(velocity, TIMESTEP));
    }
    
    public Vector2 ApplyGravity(Vector2 velocity, float time)
    {
        return new Vector2(velocity.x, velocity.y - (GRAVITY * time));
    }

}
