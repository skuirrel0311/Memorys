using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetChildGameObject : Action
{
    public SharedGameObject targetGameObject;
    private Transform targetTransform;
    public SharedInt index;
    public SharedGameObject returnObject;

    public override void OnStart()
    {
        if(targetGameObject.Value == null)
        {
            targetGameObject = gameObject;
        }

        targetTransform = targetGameObject.Value.transform;

        if(index == null)
        {
            index.SetValue(0);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(targetTransform == null)
        {
            return TaskStatus.Failure;
        }

        returnObject = targetGameObject.Value.transform.GetChild(index.Value).gameObject;
        
        return returnObject == null ? TaskStatus.Failure :TaskStatus.Success;
    }
}
