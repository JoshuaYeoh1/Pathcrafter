using UnityEngine;

[RequireComponent(typeof(SimpleVehicle))]

public class SeekFleeBehaviour : MonoBehaviour
{
    SimpleVehicle vehicle;

    void Awake()
    {
        vehicle = GetComponent<SimpleVehicle>();
    }

    void Update()
    {
        vehicle.Steer(MoveVector());
    }

    ////////////////////////////////////////////////////////////////////////////
    
    public Transform target;

    [Header("Seek")]
    public bool arrival=true;
    public float slowingRange=1;
    public float stoppingRange=.05f;
    
    [Header("Flee")]
    public bool flee;

    Vector3 MoveVector()
    {
        if(!target) return Vector3.zero;

        Vector3 targetDir = GetTargetDir();

        float distance = Vector3.Distance(target.position, transform.position);

        Vector3 desiredVelocity;

        if(arrival)
        {
            if(distance <= stoppingRange)
            {
                desiredVelocity = Vector3.zero;
            }
            else if(distance <= slowingRange)
            {
                float targetSpeed = vehicle.MaxSpeed * (distance / slowingRange);

                desiredVelocity = targetSpeed * targetDir;
            }
            else
            {
                desiredVelocity = vehicle.MaxSpeed * targetDir;
            }
        }
        else
        {
            desiredVelocity = vehicle.MaxSpeed * targetDir;
        }

        return desiredVelocity;
    }

    Vector3 GetTargetDir()
    {
        if(!flee)
        {
            return GetDir(target.position, transform.position);
        }
        else
        {
            return GetDir(transform.position, target.position);
        }
    }

    Vector3 GetDir(Vector3 targetPos, Vector3 selfPos)
    {
        return (targetPos - selfPos).normalized;
    }
}