using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Body : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;      // Visible in the Unity Editor
    [SerializeField] private Vector3 acceleration;  // Visible in the Unity Editor
    private Rigidbody rb;

    void Awake()
    {
        // Get the Rigidbody component and disable default gravity
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable Unity's gravity
    }

    void FixedUpdate()
    {
        // Update velocity based on acceleration
        velocity += acceleration * Time.fixedDeltaTime;

        // Apply the velocity to the Rigidbody
        rb.velocity = velocity;
    }

    // Apply force to the body (this method calculates acceleration internally)
    public void ApplyForce(Vector3 force)
    {
        // Use double for precision when dividing force by mass
        Vector3 preciseAcceleration = new Vector3(
            (float)((double)force.x / (double)rb.mass),
            (float)((double)force.y / (double)rb.mass),
            (float)((double)force.z / (double)rb.mass)
        );

        Debug.Log(preciseAcceleration);
        acceleration += preciseAcceleration; // Apply the precise acceleration
    }


    // Set the velocity of the body
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    // Get the velocity of the body
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    // Set the mass of the body
    public void SetMass(float newMass)
    {
        rb.mass = newMass;
    }

    // Get the mass of the body
    public float GetMass()
    {
        return rb.mass;
    }

    // Get the acceleration of the body
    public Vector3 GetAcceleration()
    {
        return acceleration;
    }
}
