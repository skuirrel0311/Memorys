using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FloatPlusFloat : Action
{
    public SharedFloat floatValue;
    public SharedFloat result;
    public bool IsMainus = false;

    public override TaskStatus OnUpdate()
    {
        if (floatValue == null || result == null) return TaskStatus.Failure;

        if (IsMainus) floatValue.SetValue(floatValue.Value * -1.0f);
        float temp = floatValue.Value + result.Value;
        result.SetValue(temp);
        return TaskStatus.Success;
    }
}
