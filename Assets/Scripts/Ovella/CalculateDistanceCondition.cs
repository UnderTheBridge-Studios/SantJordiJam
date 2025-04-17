using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CalculateDIstance", story: "Distance between [Self] and [Drac] is [Operator] [Range]", category: "Conditions", id: "4b43b650304368002f6eab558da00ad9")]
public partial class CalculateDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Drac;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
    [SerializeReference] public BlackboardVariable<int> Range;

    public System.Action Interrupt;

    public override bool IsTrue()
    {
        if (Self == null || Drac == null)
            return false;

        float distance = Math.Abs(Vector3.Distance(Self.Value.transform.position, Drac.Value.transform.position));
        return ConditionUtils.Evaluate(distance, Operator, Range.Value);
    }
}
