using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsCalculations<T> where T : IBody
{
    // Define the scaling factor (1 Unity unit = 1,000,000 meters)

    public static Vector3 CalculateGravitionalPull (T body1, T body2, float G)
    {
        // Calculate the direction vector between the two bodies
        Vector3 direction = body2.GetPosition() - body1.GetPosition();
        
        // Calculate the squared distance between the two bodies (avoiding the expensive square root operation)
        float sqrDistance = direction.sqrMagnitude;

        // Normalize the direction vector
        direction.Normalize();

        // Calculate the gravitational force magnitude using the simplified gravitational constant and the squared distance
        return direction * G *  body2.GetMass() / sqrDistance;
    }
}
