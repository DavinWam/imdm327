using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    public Vector3[] initialPositions; // Array to hold initial positions of the bodies
    public Vector3[] initialVelocities; // Array to hold initial velocities of the bodies
    public float[] masses; // Array to hold masses of the bodies
    public GameObject bodyPrefab; // Prefab with the Body script attached

    private List<Body> bodies = new List<Body>();

    public void AddBody(Body body)
    {
        bodies.Add(body);
    }

    void Start()
    {
        // Initialize the bodies with positions, velocities, and masses from the arrays
        for (int i = 0; i < initialPositions.Length; i++)
        {
            Vector3 position = i < initialPositions.Length ? initialPositions[i] : Vector3.zero;
            Vector3 velocity = i < initialVelocities.Length ? initialVelocities[i] : Vector3.zero;
            float mass = i < masses.Length ? masses[i] : 1.0f;

            // Spawn the prefab and set the body properties
            GameObject bodyObject = Instantiate(bodyPrefab, position, Quaternion.identity);
            Body body = bodyObject.GetComponent<Body>();

            // Initialize the body
            bodyObject.GetComponent<Transform>().position = position;
            body.SetVelocity(velocity);
            body.SetMass(mass);

            // Add the body to the simulation
            AddBody(body);
        }
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        // Iterate over all bodies and calculate gravitational forces
        for (int i = 0; i < bodies.Count; i++)
        {
            Vector3 totalForce = Vector3.zero;

            for (int j = 0; j < bodies.Count; j++)
            {
                if (i != j)
                {
                    // Calculate the gravitational force between body i and body j
                    Vector3 force = PhysicsCalculations.CalculateGravitationalForce(bodies[i], bodies[j]);
                    Debug.Log($"Force on body {i} from body {j}: {force}");
                    totalForce += force;

                }
            }

            // Apply the total gravitational force to the body
            bodies[i].ApplyForce(totalForce);
        }

        // No need to manually update positions, as the Body class handles movement via Rigidbody
    }


    // Restore the Gizmos for visualization
    void OnDrawGizmos()
    {
    if (initialPositions != null && !Application.isPlaying) // Only show Gizmos in edit mode
    {
        for (int i = 0; i < initialPositions.Length; i++)
        {
            // Draw the spawn position of the body (initial position)
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(initialPositions[i], 0.5f); // Draw sphere at spawn position
        }
    }
        if (bodies != null)
        {
            foreach (Body body in bodies)
            {
                if (body != null)
                {

                    // Draw the velocity vector
                    Gizmos.color = Color.green;
                    Vector3 normalizedVelocity = body.GetVelocity().normalized; // Normalize the velocity
                    Gizmos.DrawLine(body.transform.position, body.transform.position + normalizedVelocity);

                    // Draw the acceleration vector
                    Gizmos.color = Color.red;
                    Vector3 normalizedAcceleration = body.GetAcceleration().normalized*5; // Normalize the acceleration
                    Gizmos.DrawLine(body.transform.position, body.transform.position + normalizedAcceleration);
                }
            }
        }
    }
}
