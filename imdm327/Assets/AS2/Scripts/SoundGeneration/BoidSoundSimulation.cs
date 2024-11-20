using System.Collections.Generic;
using UnityEngine;

public class BoidSoundSimulation
{
    private BoidSpawner spawner;
    
    // Weights for tension calculation
    private const float separationWeight = 1.0f;
    private const float velocityWeight = 0.5f;
    private const float alignmentWeight = 0.75f;
    private const float accelerationWeight = 0.6f;

    public BoidSoundSimulation(BoidSpawner spawner)
    {
        this.spawner = spawner;
    }

    public float CalculateTension()
    {
        float averageSeparation = CalculateAverageSeparation(); // Higher separation increases tension
        float averageVelocity = CalculateAverageVelocity(); // Higher velocity contributes to tension
        float alignment = CalculateAlignment(); // Low alignment increases tension
        float acceleration = CalculateAverageAcceleration(); // Rapid changes contribute to tension

        // Combine these metrics into a single tension value
        float tension = (averageSeparation * separationWeight) +
                        (averageVelocity * velocityWeight) +
                        ((1f - alignment) * alignmentWeight) +  // Less alignment adds to tension
                        (acceleration * accelerationWeight);

        return tension;
    }

    private float CalculateAlignment()
    {
        Vector3 averageDirection = Vector3.zero;
        foreach (var boid in spawner.Boids)
        {
            averageDirection += boid.Velocity.normalized; // Sum normalized velocities
        }

        if (spawner.Boids.Count > 0)
        {
            averageDirection /= spawner.Boids.Count;
        }

        // Return the magnitude of the average direction vector as a measure of alignment
        return averageDirection.magnitude;
    }

    private float CalculateAverageAcceleration()
    {
        float totalAcceleration = 0f;
        foreach (var boid in spawner.Boids)
        {
            totalAcceleration += (boid.Velocity - boid.PreviousVelocity).magnitude;
        }

        return totalAcceleration / spawner.Boids.Count;
    }

    // Calculates the average separation distance between all pairs of boids
    private float CalculateAverageSeparation()
    {
        float totalSeparation = 0f;
        int pairCount = 0;
        int boidCount = spawner.Boids.Count;

        for (int i = 0; i < boidCount; i++)
        {
            for (int j = i + 1; j < boidCount; j++)
            {
                float distance = Vector3.Distance(spawner.Boids[i].transform.position,
                                                  spawner.Boids[j].transform.position);
                totalSeparation += distance;
                pairCount++;
            }
        }

        return pairCount > 0 ? totalSeparation / pairCount : 0f;
    }

    // Calculates the average velocity (speed) of all boids
    private float CalculateAverageVelocity()
    {
        float totalSpeed = 0f;
        int boidCount = spawner.Boids.Count;

        foreach (var boid in spawner.Boids)
        {
            totalSpeed += boid.Velocity.magnitude;
        }

        return boidCount > 0 ? totalSpeed / boidCount : 0f;
    }
}
