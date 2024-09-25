using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IBody
{
    public Vector3 initialVelocity;
    public Vector3 velocity;
    public float mass;
    [HideInInspector] public float radius;
    

    [HideInInspector]public Rigidbody rb;
    [HideInInspector]public Vector3 lastAcceleration; // Store the acceleration for the Gizmo




    void Awake()
    {
        FindRB();
        rb.useGravity = false; // We are simulating custom gravity
        rb.mass = mass;



        radius = transform.localScale.x / 2;

        // Get the TrailRenderer component and set the start width to the diameter
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.startWidth = radius * 2;  // Set the start width of the trail

        velocity = initialVelocity; // Set the initial velocity for the Rigidbody
    }

    // Calculate the gravitational force exerted on this body by all other bodies


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

    public void FindRB()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>(); // Add a Rigidbody if it doesn't exist
        }
    }

}
