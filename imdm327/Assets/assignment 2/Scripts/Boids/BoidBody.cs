using UnityEngine;

public class BoidBody : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public Vector3 PreviousVelocity { get; private set; } // Store the previous velocity
    public float MaxSpeed = 5.0f;

    public void Initialize(Vector3 initialVelocity)
    {
        Velocity = initialVelocity;
        PreviousVelocity = Velocity; // Initialize PreviousVelocity to the initial velocity
    }

    public void UpdatePosition(Vector3 acceleration)
    {
        PreviousVelocity = Velocity; // Store the current velocity before updating

        Velocity += acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);
        transform.position += Velocity * Time.deltaTime;

        if (Velocity != Vector3.zero)
        {
            transform.forward = Velocity.normalized; // Optional: Rotate to face movement direction
        }
    }
}
