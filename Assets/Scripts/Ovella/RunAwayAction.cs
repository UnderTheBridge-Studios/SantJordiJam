using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RunAway", story: "[Self] run away from [Drac]", category: "Action", id: "c1e23c704e6bd2dc0b9086c07d940663")]
public partial class RunAwayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Drac;
    [SerializeReference] public BlackboardVariable<float> ThresholdDistance = new BlackboardVariable<float>(10f);
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(3.0f);
    [SerializeReference] public BlackboardVariable<Vector3> Min;
    [SerializeReference] public BlackboardVariable<Vector3> Max;

    private Vector3 newSelfPosition;
    private Vector3 dracPosition;
    private Vector3 direction;

    protected override Status OnStart()
    {
        CheckPositions();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        CheckPositions();

        // Get direction
        direction = Self.Value.transform.position - dracPosition;
        direction.y = 0.0f;

        if (direction.magnitude > ThresholdDistance)
            return Status.Success;

        direction.Normalize();
        newSelfPosition += direction * (Speed * Time.deltaTime);

        if (newSelfPosition.x < Min.Value.x || newSelfPosition.x > Max.Value.x)
        {
            newSelfPosition.x = Self.Value.transform.position.x;
        }

        if (newSelfPosition.z > Min.Value.z || newSelfPosition.z < Max.Value.z)
        {
            newSelfPosition.z = Self.Value.transform.position.z;
        }

        Self.Value.transform.position = newSelfPosition;

        //Vector3 velocity = direction * Speed;
        //selfController.SimpleMove(velocity);

        // Rotate
        float angle = Vector3.SignedAngle(newSelfPosition, direction, Vector3.up);
        Self.Value.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.3f);

        return Status.Running;
    }

    private void CheckPositions()
    {
        newSelfPosition = Self.Value.transform.position;
        dracPosition = Drac.Value.transform.position;
    }
}

