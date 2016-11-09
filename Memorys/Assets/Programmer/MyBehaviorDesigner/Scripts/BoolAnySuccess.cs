using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BoolAnySuccess : Conditional
{
    public SharedBool[] boolValues;

    public override TaskStatus OnUpdate()
    {
        foreach(SharedBool v in boolValues)
        {
            if (v.Value) return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
