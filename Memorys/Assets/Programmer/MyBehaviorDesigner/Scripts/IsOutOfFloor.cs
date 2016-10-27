using UnityEngine;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsOutOfFloor : Conditional
{
    public SharedVector3 center;
    public SharedVector3 size;
    public LayerMask objectLayerMask;

    public bool IsViewGUI = false;

    public override TaskStatus OnUpdate()
    {
        Collider[] cols = Physics.OverlapBox(center.Value, size.Value * 0.5f, Quaternion.identity, objectLayerMask);

        if (cols.Any(n => n.gameObject.Equals(gameObject)))
            return TaskStatus.Failure;
        else
            return TaskStatus.Success;
    }

    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!IsViewGUI) return;
        if (Owner == null || size == null)
        {
            return;
        }
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireCube(center.Value, size.Value);
        UnityEditor.Handles.color = oldColor;
#endif
    }
}
