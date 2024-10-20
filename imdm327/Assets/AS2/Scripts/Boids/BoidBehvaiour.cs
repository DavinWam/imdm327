using System.Collections.Generic;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    // Reference to the BoidSettings ScriptableObject
    public BoidSettings settings;

    private BoidBody agent;
    private BoidSpawner spawner;

    void Start()
    {
        agent = GetComponent<BoidBody>();
        spawner = FindObjectOfType<BoidSpawner>();

        // Apply settings to the agent
        agent.MaxSpeed = settings.MaxSpeed;
    }

    void Update()
    {
        Vector3 acceleration = CalculateSteeringForces();
        agent.UpdatePosition(acceleration);
    }

    // Calculate the combined steering forces
    private Vector3 CalculateSteeringForces()
    {
        Vector3 steeringForce = Vector3.zero;

        if (settings.EnableSeparation)
        {
            Vector3 separation = CalculateSeparation() * settings.SeparationStrength;
            steeringForce += separation;
        }

        if (settings.EnableAlignment)
        {
            Vector3 alignment = CalculateAlignment() * settings.AlignmentStrength;
            steeringForce += alignment;
        }

        if (settings.EnableCohesion)
        {
            Vector3 cohesion = CalculateCohesion() * settings.CohesionStrength;
            steeringForce += cohesion;
        }

        // If you have a containment force, you can include it as well
        Vector3 containment = CalculateContainment() * spawner.ContainmentStrength;
        steeringForce += containment;

        return steeringForce;
    }

    // Separation: Avoid crowding neighbors
    private Vector3 CalculateSeparation()
    {
        Vector3 steering = Vector3.zero;
        int count = 0;

        foreach (BoidBody other in spawner.Boids)
        {
            if (other != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, other.transform.position);
                if (distance > 0 && distance < settings.SeparationRadius)
                {
                    Vector3 difference = agent.transform.position - other.transform.position;
                    difference /= distance; // Weight by distance
                    steering += difference;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            steering /= count;
        }

        return steering.normalized;
    }

    // Alignment: Steer towards the average heading of neighbors
    private Vector3 CalculateAlignment()
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        foreach (BoidBody other in spawner.Boids)
        {
            if (other != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, other.transform.position);
                if (distance < settings.AlignmentRadius)
                {
                    averageVelocity += other.Velocity;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            averageVelocity /= count;
            Vector3 desiredVelocity = averageVelocity.normalized * agent.MaxSpeed;
            Vector3 steering = desiredVelocity - agent.Velocity;
            return steering.normalized;
        }

        return Vector3.zero;
    }

    // Cohesion: Steer towards the average position of neighbors
    private Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (BoidBody other in spawner.Boids)
        {
            if (other != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, other.transform.position);
                if (distance < settings.CohesionRadius)
                {
                    centerOfMass += other.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            Vector3 direction = centerOfMass - agent.transform.position;
            Vector3 desiredVelocity = direction.normalized * agent.MaxSpeed;
            Vector3 steering = desiredVelocity - agent.Velocity;
            return steering.normalized;
        }

        return Vector3.zero;
    }
private Vector3 CalculateContainment()
{
    Vector3 steering = Vector3.zero;
    Vector3 position = agent.transform.position;
    Vector3 halfBounds = spawner.SimulationBounds / 2;
    Vector3 center = spawner.transform.position; // Assuming the center is at the manager's position

    // X-axis containment
    float distanceX = Mathf.Abs(position.x - center.x);
    if (distanceX > halfBounds.x - spawner.ContainmentThreshold)
    {
        float directionX = center.x - position.x;
        steering.x = directionX;
    }

    // Y-axis containment
    float distanceY = Mathf.Abs(position.y - center.y);
    if (distanceY > halfBounds.y - spawner.ContainmentThreshold)
    {
        float directionY = center.y - position.y;
        steering.y = directionY;
    }

    // Z-axis containment
    float distanceZ = Mathf.Abs(position.z - center.z);
    if (distanceZ > halfBounds.z - spawner.ContainmentThreshold)
    {
        float directionZ = center.z - position.z;
        steering.z = directionZ;
    }

    return steering.normalized;
}

}
