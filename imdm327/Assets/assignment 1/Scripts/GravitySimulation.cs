using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{

    public List<CelestialBody> bodies = new List<CelestialBody>();
    public const float G = 0.0001f;
    public const float physicsTimeStep = 0.01f;

    public float timeScale =1f;
    void Update(){
        Time.timeScale = timeScale;
    }
    void FixedUpdate()
    {
        // First loop: Each body calculates its forces from other bodies and updates its velocity
        foreach (CelestialBody body in bodies)
        {
            body.CalculateAcceleration(G); // Calculate all forces acting on the body
        }
        foreach (CelestialBody body in bodies)
        {
            body.UpdatePosition(); 
        }

    }

}
