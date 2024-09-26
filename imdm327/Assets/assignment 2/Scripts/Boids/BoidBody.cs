using UnityEngine;

public class BoidBody : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public float MaxSpeed = 5.0f;


    public void Initialize(Vector3 initialVelocity)
    {
        Velocity = initialVelocity;
    }

    public void UpdatePosition(Vector3 acceleration)
    {
        Velocity += acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);
        transform.position += Velocity * Time.deltaTime;

        if (Velocity != Vector3.zero)
        {
            transform.forward = Velocity.normalized; // Optional: Rotate to face movement direction
        }
    }
}
