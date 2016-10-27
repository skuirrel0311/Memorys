using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetMyCurrentTransform : Action
{
    public SharedTransform currentTransform;

    public override TaskStatus OnUpdate()
    {
        currentTransform.SetValue(transform);

        if (transform == null)
            return TaskStatus.Failure;
        else
            return TaskStatus.Success;
    }
}
