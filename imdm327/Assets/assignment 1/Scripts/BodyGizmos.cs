using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

[ExecuteInEditMode]
public class BodyDebug : MonoBehaviour
{
    public bool showForces = true; // Boolean flag to toggle gizmos

  
    void OnDrawGizmos()
    {
        if (!showForces) return; // Exit if gizmos are turned off

        // Find all bodies in the scene
        Body[] allBodies = FindObjectsOfType<Body>();

        // Draw velocity and acceleration vectors for each body
        foreach (Body body in allBodies)
        {
            if (body != null)
            {
                Renderer renderer = body.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    // Get the bounds size and use it to scale the lines
                    float objectSize = renderer.bounds.size.magnitude;

                    // Draw the velocity vector, scaled by the object size
                    Gizmos.color = Color.green;
                    Vector3 normalizedVelocity = body.GetVelocity().normalized * objectSize;
                    Gizmos.DrawLine(body.transform.position, body.transform.position + normalizedVelocity);

                    // Draw the acceleration vector, scaled by the object size
                    Gizmos.color = Color.red;
                    Vector3 normalizedAcceleration = body.GetAcceleration().normalized * objectSize * 2;
                    Gizmos.DrawLine(body.transform.position, body.transform.position + normalizedAcceleration);
                }
            }
        }
    }

}
