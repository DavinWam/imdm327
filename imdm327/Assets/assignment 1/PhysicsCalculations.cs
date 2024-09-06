using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsCalculations
{
    public static Vector3 CalculateGravitationalForce(Body body1, Body body2)
    {
        float G = 6.67430e-11f; // Gravitational constant
        Vector3 direction = body2.GetComponent<Transform>().position - body1.GetComponent<Transform>().position;
        float distance = direction.magnitude;

        // Normalize the direction vector
        direction.Normalize();

        // Calculate the gravitational force magnitude
        float forceMagnitude = G * (body1.GetMass() * body2.GetMass()) / (distance * distance);
        
        return direction * forceMagnitude;
    }
}
