using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveAgent", story: "Moves [Self] to [TargetPosition] and changes [IsMoving] value", category: "Action", id: "7523f7c4f63362e7c94073fec6fcaafa")]
public partial class MoveAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Location;
    [SerializeReference] public BlackboardVariable<Vector3> LocationPadding;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

    protected override Status OnStart()
    {
        if (Agent.Value == null || Location.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Location.Value == null)
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
        Agent.Value.transform.position = agentPosition;

        // Look at the target.
        //Agent.Value.transform.forward = toDestination;
        float angle = Vector2.SignedAngle(toDestination, Vector2.up);
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        Agent.Value.transform.rotation = Quaternion.Lerp(Agent.Value.transform.rotation, targetRotation, 0.05f * Time.deltaTime);

        return Status.Running;
    }


    private Status Initialize()
    {
        if (GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition) <= DistanceThreshold)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    private float GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition)
    {
        agentPosition = Agent.Value.transform.position;
        locationPosition = Location.Value + LocationPadding;

        Debug.Log("AgentPosition: " + agentPosition);
        Debug.Log("LocationPosition: " + locationPosition);
        return Vector3.Distance(new Vector3(agentPosition.x, locationPosition.y, agentPosition.z), locationPosition);
    }
}

