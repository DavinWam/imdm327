using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Settings/BoidSettings")]
public class BoidSettings : ScriptableObject
{
    // Boid perception radii
    public float SeparationRadius = 1.0f;
    public float AlignmentRadius = 2.0f;
    public float CohesionRadius = 2.5f;

    // Strength multipliers for each behavior
    public float SeparationStrength = 1.5f;
    public float AlignmentStrength = 1.0f;
    public float CohesionStrength = 0.5f;

    // Maximum speed for boids
    public float MaxSpeed = 5.0f;

    // Booleans to enable/disable each force
    public bool EnableSeparation = true;
    public bool EnableAlignment = true;
    public bool EnableCohesion = true;
}
