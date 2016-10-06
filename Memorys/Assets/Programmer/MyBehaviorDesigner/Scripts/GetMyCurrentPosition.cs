using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetMyCurrentPosition : Action
{
    public SharedVector3 currentPosition;

    public override TaskStatus OnUpdate()
    {
        currentPosition.Value = transform.position;

        if (transform == null)
            return TaskStatus.Failure;
        else
            return TaskStatus.Success;
    }
}
