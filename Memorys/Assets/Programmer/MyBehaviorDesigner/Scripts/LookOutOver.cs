using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class LookOutOver : Action
{
    private enum TargetDirection { forward, back, left, right}
    TargetDirection currentTrargetDirection;
    List<TargetDirection> directionPattenList = new List<TargetDirection>();
    Vector3[] directionVec = new Vector3[4];

    Quaternion targetRotation;
    public int currentIndex;
    public SharedFloat rotationSpeed;

    public override void OnStart()
    {
        directionVec[(int)TargetDirection.forward] = transform.forward;
        directionVec[(int)TargetDirection.left] = transform.rotation * Vector3.left;
        directionVec[(int)TargetDirection.right] = directionVec[(int)TargetDirection.left] * -1;
        directionVec[(int)TargetDirection.back] = directionVec[(int)TargetDirection.forward] * -1;
        AddPatten();
        currentIndex = 0;
        currentTrargetDirection = directionPattenList[currentIndex];
        targetRotation = Quaternion.LookRotation(directionVec[(int)currentTrargetDirection]);
    }

    private void AddPatten()
    {
        directionPattenList.Add(TargetDirection.left);
        directionPattenList.Add(TargetDirection.forward);
        directionPattenList.Add(TargetDirection.right);
        directionPattenList.Add(TargetDirection.forward);
        directionPattenList.Add(TargetDirection.back);
        directionPattenList.Add(TargetDirection.forward);
    }

    public override TaskStatus OnUpdate()
    {
        RotateTowards();
        if (directionPattenList.Count <= currentIndex)
        {
            Debug.Log("complete");
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        directionPattenList.Clear();
    }

    private void RotateTowards()
    {
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
        {
            currentIndex++;
            //範囲外
            if (directionPattenList.Count <= currentIndex) return;

            currentTrargetDirection = directionPattenList[currentIndex];
            targetRotation = Quaternion.LookRotation(directionVec[(int)currentTrargetDirection]);
            return;
        }
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.Value * Time.deltaTime);

        return;
    }
}
