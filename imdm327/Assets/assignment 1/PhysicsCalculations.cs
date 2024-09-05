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

        // Softening factor to avoid large forces when bodies get too close
        float epsilon = 1e-2f; // A small number to soften the gravitational force at small distances

        // Calculate the modified force magnitude with softening
        direction.Normalize();
        float forceMagnitude = G * (body1.GetMass() * body2.GetMass()) / (distance * distance + epsilon * epsilon);
        
        return direction * forceMagnitude;
    }
}

