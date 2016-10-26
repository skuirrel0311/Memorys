using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetMyCurrentRotation : Action
{
    public SharedVector3 currentRotation;

    public override TaskStatus OnUpdate()
    {
        currentRotation.SetValue(transform.eulerAngles);
        if (transform == null)
            return TaskStatus.Failure;
        else
            return TaskStatus.Success;
    }
}
