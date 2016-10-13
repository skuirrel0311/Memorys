using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class DestroyGameObject : Action
{
    public SharedGameObject targetObject;

    public override void OnStart()
    {
        if (targetObject.Value == null)
        {
            targetObject.Value = gameObject;
        }

        BehaviorTree tree = targetObject.Value.GetComponent<BehaviorTree>();
        if (tree != null) tree.DisableBehavior();

        GameObject.Destroy(targetObject.Value);
    }
}
