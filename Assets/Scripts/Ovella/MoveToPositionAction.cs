using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;
using System.Runtime.CompilerServices;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToPosition", story: "Move [Self] to [TargetPosition]", category: "Action", id: "3391ffcaadad1b7d59f2b72bf3d0300f")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

    protected override Status OnStart()
    {
        if (Self.Value == null || TargetPosition.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || TargetPosition.Value == null)
        {
            return Status.Failure;
        }

        Vector3 agentPosition, locationPosition;
        float distance = GetDistanceToLocation(out agentPosition, out locationPosition);
        if (distance <= DistanceThreshold)
        {
            return Status.Success;
        }

        float speed = Speed;

        if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
        {
            float ratio = distance / SlowDownDistance;
            speed = Mathf.Max(0.1f, Speed * ratio);
        }

        Vector3 toDestination = locationPosition - agentPosition;
        toDestination.y = 0.0f;
        toDestination.Normalize();
        agentPosition += toDestination * (speed * Time.deltaTime);
        Self.Value.transform.position = agentPosition;
        
        //Vector3 velocity = toDestination * speed;
        //selfController.SimpleMove(velocity);

        // Look at the target.
        float angle = Vector3.SignedAngle(agentPosition, toDestination, Vector3.up);
        float currentAngle = Mathf.LerpAngle(0, angle, Time.time);
        Self.Value.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.3f);

        return Status.Running;
    }

    private Status Initialize()
    {
        if (GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition) <= DistanceThreshold)
            return Status.Success;

        return Status.Running;
    }

    private float GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition)
    {
        agentPosition = Self.Value.transform.position;
        locationPosition = TargetPosition.Value;
        return Vector3.Distance(new Vector3(agentPosition.x, locationPosition.y, agentPosition.z), locationPosition);
    }
}

