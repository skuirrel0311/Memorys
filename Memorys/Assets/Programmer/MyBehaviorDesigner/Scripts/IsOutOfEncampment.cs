using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/// <summary>
/// 距離だけをみて、ある地点から離れていたらSuccessを返す
/// </summary>
public class IsOutOfEncampment : Conditional
{
    public SharedVector3 centerPosition;
    public SharedFloat distance;
    public bool IsViewGUI = false;

    public override TaskStatus OnUpdate()
    {
        if (transform == null) return TaskStatus.Failure;

        float temp = Vector3.Distance(centerPosition.Value ,transform.position);

        if (temp < distance.Value)
            return TaskStatus.Failure;
        else
            return TaskStatus.Success;
    }

    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!IsViewGUI) return;
        if (Owner == null)
        {
            return;
        }
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(centerPosition.Value, Vector3.up, distance.Value);
        UnityEditor.Handles.color = oldColor;
#endif
    }
}
