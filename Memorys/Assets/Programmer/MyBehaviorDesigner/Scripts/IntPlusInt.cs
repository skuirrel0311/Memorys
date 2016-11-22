using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IntPlusInt : Action 
{
    public SharedInt intValue;
    public SharedInt result;
    public bool IsMainus = false;

    public override TaskStatus OnUpdate()
    {
        if (intValue == null || result == null) return TaskStatus.Failure;

        if (IsMainus) intValue.SetValue(intValue.Value * -1);
        int temp = intValue.Value + result.Value;
        result.SetValue(temp);
        return TaskStatus.Success;
    }
}
