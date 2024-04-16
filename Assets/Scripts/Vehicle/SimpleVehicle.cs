using UnityEngine;

public class SimpleVehicle : MonoBehaviour
{
    public float mass = 1.0f;
    public float maxForce = 0.01f;
    public float maxSpeed = 2.0f;
    [HideInInspector] public float defSpeed;

    Vector3 acceleration;

    public float Mass { get => Mathf.Max(0.001f, mass); }

    public float MaxForce { get => maxForce; }
    public float MaxSpeed { get => maxSpeed; }
    public Vector3 Position { get => transform.position; }

    public Vector3 ForwardHeading
    { get => transform.up; set { transform.up = value; } }

    public Vector3 CurrentVelocity { get; set; }
    public Vector3 TurnVelocity { get; set; } // orientation

    // Instantly teleport the vehicle to a different position
    public void Teleport(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void Steer(Vector3 desiredVelocity)
    {
        Vector3 force = CalculateSteeringForce(desiredVelocity);
        acceleration += force / Mass;
    }

    Vector3 CalculateSteeringForce(Vector3 desiredVelocity)
    {
        var steerVector = desiredVelocity - CurrentVelocity;
        var clampedForce = Vector3.ClampMagnitude(steerVector, maxForce);
        return clampedForce;
    }

    void ApplyMove()
    {
        CurrentVelocity = Vector3.ClampMagnitude(CurrentVelocity + acceleration, MaxSpeed);
        acceleration *= 0;

        transform.Translate(CurrentVelocity * Time.deltaTime, Space.World);
    }

    void AdjustOrientation()
    {
        float speed = CurrentVelocity.magnitude;
        // Ternary operator
        // variable = condition ? <value when true> : <value when false>;
        TurnVelocity = Mathf.Approximately(speed, 0.0f) ? Vector3.zero : CurrentVelocity / speed;

        ForwardHeading = Mathf.Approximately(TurnVelocity.magnitude, 0.0f) ?
            ForwardHeading : TurnVelocity;
    }
    
    void Start()
    {
        defSpeed = maxSpeed;
    }

    void Update()
    {
        ApplyMove();
        AdjustOrientation();
    }
}