using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IBody
{
    public Vector3 initialVelocity;
    public Vector3 velocity;
    public float mass;
    [HideInInspector] public float radius;
    
    private Rigidbody rb;
    public GravitySimulation simulation;
    private Vector3 lastAcceleration; // Store the acceleration for the Gizmo

    // Main body for orbit calculations
    public Body mainBody;
    public bool useEllipticalOrbit; // Toggle for using elliptical orbit calculation

    void Awake()
    {
        FindRB();
        rb.useGravity = false; // We are simulating custom gravity
        rb.mass = mass;


        simulation = FindObjectOfType<GravitySimulation>();
        radius = transform.localScale.x / 2;

        // Get the TrailRenderer component and set the start width to the diameter
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.startWidth = radius * 2;  // Set the start width of the trail

        if (useEllipticalOrbit && mainBody != null)
        {
            SetInitialVelocityForEllipticalOrbit();
        }
        velocity = initialVelocity; // Set the initial velocity for the Rigidbody
    }

    // Calculate the gravitational force exerted on this body by all other bodies
    public void CalculateAcceleration(float gravitationalConstant)
    {
        Vector3 totalAcceleration = Vector3.zero;

        foreach (Body otherBody in simulation.bodies)
        {
            if (otherBody != this)
            {
                // Calculate the gravitational force between this body and the other body
                Vector3 acceleration = PhysicsCalculations<Body>.CalculateGravitionalPull(this, otherBody, gravitationalConstant);
                totalAcceleration += acceleration;
            }
        }

        // Calculate the acceleration based on the total force and mass
        lastAcceleration = totalAcceleration;

        // Apply the accumulated force as a force to the Rigidbody
        velocity += totalAcceleration * GravitySimulation.physicsTimeStep;
    }

    public void UpdatePosition()
    {
        FindRB();
        rb.MovePosition(rb.position + velocity * GravitySimulation.physicsTimeStep);
    }

    public Vector3 GetPosition()
    {
        FindRB();
        return rb.position;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public float GetMass()
    {
        return mass;
    }

    public void SetMass(float newMass)
    {
        mass = newMass;
    }

    public Vector3 GetAcceleration()
    {
        return lastAcceleration;
    }

    private void FindRB()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>(); // Add a Rigidbody if it doesn't exist
        }
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
        foreach (Body otherBody in simulation.bodies)
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
