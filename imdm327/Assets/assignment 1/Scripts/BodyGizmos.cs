using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

[ExecuteInEditMode]
public class BodyDebug : MonoBehaviour
{
    public bool showForces = true; // Boolean flag to toggle gizmos
    public int numSteps = 1000;
    public float timeStep = 0.1f;
    public bool usePhysicsTimeStep;

    public bool relativeToSun;
    public Body Sun;
    public GravitySimulation simulation;

    void Update()
    {
        if (!Application.isPlaying)
        {
            DrawOrbits();
        }
    }

    void DrawOrbits()
    {
        if (simulation)
        {
            List<Body> bodies =  FindObjectsOfType<Body>().ToList();
            List<ProjectedBody> projectedBodies = new List<ProjectedBody>(bodies.Count);
            var drawPoints = new Vector3[bodies.Count][];
            int SunIndex = 0;
            Vector3 SunInitialPosition = Vector3.zero;

            // Initialize virtual bodies (don't want to move the actual bodies)
            for (int i = 0; i < bodies.Count; i++)
            {
                projectedBodies.Add(new ProjectedBody(bodies[i]));
                if(bodies[i].useEllipticalOrbit){
                    projectedBodies[i].velocity = bodies[i].CalculateEllipticalOrbit();
                }

                drawPoints[i] = new Vector3[numSteps];

                if (bodies[i] == Sun && relativeToSun)
                {
                    SunIndex = i;
                    SunInitialPosition = projectedBodies[i].position;
                }
            }

            // Simulate
            for (int step = 0; step < numSteps; step++)
            {
                Vector3 sunPosition = (relativeToSun) ? projectedBodies[SunIndex].position : Vector3.zero;

                // Update velocities
                for (int i = 0; i < projectedBodies.Count; i++)
                {
                    projectedBodies[i].velocity += CalculateAcceleration(i, projectedBodies) * timeStep;
                }

                // Update positions
                for (int i = 0; i < projectedBodies.Count; i++)
                {
                    Vector3 newPos = projectedBodies[i].position + projectedBodies[i].velocity * timeStep;
                    projectedBodies[i].position = newPos;

                    if (relativeToSun)
                    {
                        Vector3 referenceFrameOffset = sunPosition - SunInitialPosition;
                        newPos -= referenceFrameOffset;
                    }

                    if (relativeToSun && i == SunIndex)
                    {
                        newPos = SunInitialPosition;
                    }


                    drawPoints[i][step] = newPos;   
                }
            }

            // Draw paths
            for (int bodyIndex = 0; bodyIndex < projectedBodies.Count; bodyIndex++)
            {
                Color pathColour = bodies[bodyIndex].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;

                for (int i = 0; i < drawPoints[bodyIndex].Length  - 1; i++)
                {
                    Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
                    // if (bodyIndex != SunIndex)  Debug.Log(drawPoints[bodyIndex][i]+ " " + bodyIndex);
                }

                
            }
        }
    }

    Vector3 CalculateAcceleration(int i, List<ProjectedBody> virtualBodies)
    {
        Vector3 totalAcceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Count; j++)
        {
            if (i == j)
            {
                continue;
            }

            totalAcceleration +=  PhysicsCalculations<ProjectedBody>.CalculateGravitionalPull( virtualBodies[i],virtualBodies[j] ,GravitySimulation.G); // direction * GravitySimulation.G * virtualBodies[j].mass / sqrDistance;
        }
        return totalAcceleration;
    }
    void OnValidate () {
        if (usePhysicsTimeStep) {
            timeStep = GravitySimulation.physicsTimeStep;
        }
    }



    class ProjectedBody : IBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        public ProjectedBody(Body body)
        {
            position = body.transform.position;
            velocity = body.initialVelocity;
            mass = body.GetMass();
        }
        public Vector3 GetPosition(){
            return position;
        }
        public float GetMass(){
            return mass;
        }
    }
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
