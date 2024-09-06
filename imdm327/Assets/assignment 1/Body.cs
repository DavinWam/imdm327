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

    // Reset the acceleration to zero at the start of each frame
    public void ResetAcceleration()
    {
        acceleration = Vector3.zero;
    }

    // Apply force to the body (this method calculates acceleration internally)
    public void ApplyForce(Vector3 force)
    {
        Vector3 preciseAcceleration = new Vector3(
            (float)((double)force.x / (double)rb.mass),
            (float)((double)force.y / (double)rb.mass),
            (float)((double)force.z / (double)rb.mass)
        );
        acceleration += preciseAcceleration; // Apply the precise acceleration
    }

    // Update the velocity based on the accumulated acceleration and apply it to the Rigidbody
    public void UpdateVelocity(float deltaTime)
    {
        velocity += acceleration * deltaTime;
        rb.velocity = velocity;
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
