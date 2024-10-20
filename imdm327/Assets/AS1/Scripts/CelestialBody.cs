using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : Body
{
        // Main body for orbit calculations
    public CelestialBody mainBody;
    public bool useEllipticalOrbit; // Toggle for using elliptical orbit calculation
    public GravitySimulation simulation;
    // Start is called before the first frame update
    void Start()
    {
        simulation = FindObjectOfType<GravitySimulation>();
        
        if (useEllipticalOrbit && mainBody != null)
        {
            SetInitialVelocityForEllipticalOrbit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CalculateAcceleration(float gravitationalConstant)
    {
        Vector3 totalAcceleration = Vector3.zero;

        foreach (CelestialBody otherBody in simulation.bodies)
        {
            if (otherBody != this)
            {
                // Calculate the gravitational force between this body and the other body
                Vector3 acceleration = PhysicsCalculations<CelestialBody>.CalculateGravitionalPull(this, otherBody, gravitationalConstant);
                totalAcceleration += acceleration;
            }
        }

        // Calculate the acceleration based on the total force and mass
        lastAcceleration = totalAcceleration;

        // Apply the accumulated force as a force to the Rigidbody
        velocity += totalAcceleration * GravitySimulation.physicsTimeStep;
    }
    // Function to set the initial velocity for elliptical orbit
    public void SetInitialVelocityForEllipticalOrbit()
    {
        if (simulation.bodies == null || simulation.bodies.Count == 0)
        {
            Debug.LogWarning("No bodies available for elliptical orbit calculation.");
            return ;
        }
        Vector3 totalAcceleration = CalculateEllipticalOrbit();

        // Set the velocity based on the sum of all gravitational influences
        initialVelocity = totalAcceleration;
    }

    public Vector3 CalculateEllipticalOrbit()
    {
        Vector3 totalAcceleration = Vector3.zero;
        Vector3 position = transform.position;
        float G = GravitySimulation.G;

        // Iterate over all bodies in the simulation
        foreach (CelestialBody otherBody in simulation.bodies)
        {
            if (otherBody != this) // Exclude the current body
            {
                // Calculate the direction and distance between this body and the other body
                Vector3 direction = otherBody.GetPosition() - position;
                float distance = direction.magnitude;

                // Calculate the gravitational influence of this body
                float velocityMagnitude = Mathf.Sqrt(G * otherBody.GetMass() / distance);

                // Find the direction perpendicular to the current direction vector (for stable orbit)
                Vector3 perpendicularDirection = Vector3.Cross(direction.normalized, Vector3.up).normalized;

                // Add the calculated velocity to the total velocity
                totalAcceleration += perpendicularDirection * velocityMagnitude;
            }
        }
        return totalAcceleration;
    }
}
