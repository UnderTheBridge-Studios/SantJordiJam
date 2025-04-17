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
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(3.0f);

    private Vector3 selfPosition;
    private Vector3 dracPosition;
    private Vector3 direction;

    private bool firstUpdate = true;

    protected override Status OnStart()
    {
        CheckPositions();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        CheckPositions();

        // Get direction
        direction = selfPosition - dracPosition;
        direction.y = 0.0f;
        direction.Normalize();
        selfPosition += direction * (Speed * Time.deltaTime);

        // Move
        Self.Value.transform.position = selfPosition;

        // Rotate
        float angle = Vector3.SignedAngle(selfPosition, direction, Vector3.up);
        Self.Value.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.3f);

        return Status.Running;
    }

    private void CheckPositions()
    {
        selfPosition = Self.Value.transform.position;
        dracPosition = Drac.Value.transform.position;
    }
}

