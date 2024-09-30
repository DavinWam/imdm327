using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSoundManager : MonoBehaviour
{
    public FMSynth padSynth1;
    public FMSynth padSynth2;
    public FMSynth gongSynth;
    public FMSynth oboeSynth;

    private BoidSpawner spawner;

    // Thresholds for clustering and direction change
    public float clusteringSeparationThreshold = 2.0f;
    public float directionChangeThreshold = 30.0f; // in degrees

    void Start()
    {
        // Get the BoidSpawner component from the same GameObject
        spawner = GetComponent<BoidSpawner>();
        if (spawner == null)
        {
            Debug.LogError("BoidSoundManager requires a BoidSpawner component on the same GameObject.");
        }
    }

    void Update()
    {
        if (spawner == null) return;

        // Calculate simulation metrics
        float averageSeparation = CalculateAverageSeparation();
        float averageVelocity = CalculateAverageVelocity();

        // Adjust pad modulationDepth based on average separation
        padSynth1.operators[0].modulationDepth = Mathf.Clamp(averageSeparation * 100f, 50f, 500f);
        padSynth2.operators[0].modulationDepth = Mathf.Clamp(averageSeparation * 100f, 50f, 500f);

        // Adjust gong volume based on clustering
        gongSynth.operators[0].volume = BoidsAreClustering() ? 1.0f : 0.0f;

        // Adjust oboe frequencyMultiplier based on velocity
        oboeSynth.operators[0].frequencyMultiplier = Mathf.Clamp(averageVelocity / 2f, 1.0f, 3.0f);

        // Trigger sounds based on events
        if (BoidsAreClustering())
        {
            gongSynth.NoteOn();
        }
        else
        {
            gongSynth.NoteOff();
        }

        if (BoidsChangeDirection())
        {
            oboeSynth.NoteOn();
        }
        else
        {
            oboeSynth.NoteOff();
        }
    }

    /// <summary>
    /// Calculates the average separation distance between all pairs of boids.
    /// </summary>
    /// <returns>Average separation distance.</returns>
    float CalculateAverageSeparation()
    {
        float totalSeparation = 0f;
        int pairCount = 0;
        int boidCount = spawner.Boids.Count;

        for (int i = 0; i < boidCount; i++)
        {
            for (int j = i + 1; j < boidCount; j++)
            {
                float distance = Vector3.Distance(spawner.Boids[i].GetComponent<Transform>().position, 
                    spawner.Boids[j].GetComponent<Transform>().position);
                totalSeparation += distance;
                pairCount++;
            }
        }

        return pairCount > 0 ? totalSeparation / pairCount : 0f;
    }

    /// <summary>
    /// Calculates the average velocity (speed) of all boids.
    /// </summary>
    /// <returns>Average velocity.</returns>
    float CalculateAverageVelocity()
    {
        float totalSpeed = 0f;
        int boidCount = spawner.Boids.Count;

        foreach (var boid in spawner.Boids)
        {
            totalSpeed += boid.Velocity.magnitude;
        }

        return boidCount > 0 ? totalSpeed / boidCount : 0f;
    }

    /// <summary>
    /// Determines if the boids are clustering based on the average separation threshold.
    /// </summary>
    /// <returns>True if boids are clustering; otherwise, false.</returns>
    bool BoidsAreClustering()
    {
        float averageSeparation = CalculateAverageSeparation();
        return averageSeparation < clusteringSeparationThreshold;
    }

    /// <summary>
    /// Determines if the boids are changing direction sharply based on the direction change threshold.
    /// </summary>
    /// <returns>True if boids are changing direction; otherwise, false.</returns>
    bool BoidsChangeDirection()
    {
        // Calculate the average velocity direction
        Vector3 averageVelocity = Vector3.zero;
        int boidCount = spawner.Boids.Count;

        foreach (var boid in spawner.Boids)
        {
            averageVelocity += boid.Velocity;
        }

        averageVelocity = boidCount > 0 ? averageVelocity.normalized : Vector3.zero;

        // Calculate the average angle deviation from the average velocity
        float totalAngleDeviation = 0f;

        foreach (var boid in spawner.Boids)
        {
            if (averageVelocity == Vector3.zero) continue;
            float angle = Vector3.Angle(boid.Velocity, averageVelocity);
            totalAngleDeviation += angle;
        }

        float averageAngleDeviation = boidCount > 0 ? totalAngleDeviation / boidCount : 0f;

        return averageAngleDeviation > directionChangeThreshold;
    }
}
