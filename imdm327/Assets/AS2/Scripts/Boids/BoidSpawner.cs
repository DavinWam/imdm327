using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject BoidPrefab;
    public int BoidCount = 100;
    public float SpawnRadius = 10.0f;
    public float StartingSpeed = 5.0f;

    public BoidSettings settings; // Reference to BoidSettings
    public Vector3 SimulationBounds = new Vector3(50, 50, 50); // Width, Height, Depth
    public float ContainmentThreshold = 5.0f; // Distance from the boundary to start applying the containment force
    public float ContainmentStrength = 2.0f; // Strength of the containment force

    [HideInInspector]
    public List<BoidBody> Boids = new List<BoidBody>();

    void Start()
    {
        InitializeBoids();
    }

    private void InitializeBoids()
    {
        for (int i = 0; i < BoidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * SpawnRadius;
            GameObject boidObject = Instantiate(BoidPrefab, spawnPosition, Quaternion.identity);

            BoidBody agent = boidObject.GetComponent<BoidBody>();
            Vector3 initialVelocity = Random.insideUnitSphere.normalized * StartingSpeed;
            agent.Initialize(initialVelocity);

            BoidBehavior behavior = boidObject.GetComponent<BoidBehavior>();
            behavior.settings = this.settings; // Assign settings

            Boids.Add(agent);
        }
    }
    void OnDrawGizmos()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(transform.position, SimulationBounds);
}

}
