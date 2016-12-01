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
    //回転の速さ
    public float delta = 1.0f;

    public override void OnStart()
    {
        //パターン作成
        directionVec[(int)TargetDirection.forward] = transform.forward;
        directionVec[(int)TargetDirection.left] = transform.rotation * Vector3.left;
        directionVec[(int)TargetDirection.right] = directionVec[(int)TargetDirection.left] * -1;
        directionVec[(int)TargetDirection.back] = directionVec[(int)TargetDirection.forward] * -1;
        //ランダムなパターンをセットする。
        SetRandomPatten();
        currentIndex = 0;
        currentTrargetDirection = directionPattenList[currentIndex];
        targetRotation = Quaternion.LookRotation(directionVec[(int)currentTrargetDirection]);
    }

    //時計回り
    private void SetClockWisePatten()
    {
        directionPattenList.Add(TargetDirection.left);
        directionPattenList.Add(TargetDirection.back);
        directionPattenList.Add(TargetDirection.right);
        directionPattenList.Add(TargetDirection.forward);
    }

    private void SetAntiClockWisePatten()
    {
        directionPattenList.Add(TargetDirection.right);
        directionPattenList.Add(TargetDirection.back);
        directionPattenList.Add(TargetDirection.left);
        directionPattenList.Add(TargetDirection.forward);
    }

    private void SetRandomPatten()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
            SetClockWisePatten();
        else
            SetAntiClockWisePatten();

        //for (int i = 0; i < num; i++)
        //{
        //    //0～3
        //    directionPattenList.Add((TargetDirection)Random.Range(0, 4));
        //}
    }
    
    public override TaskStatus OnUpdate()
    {
        RotateTowards();
        if (directionPattenList.Count <= currentIndex)
        {
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
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, delta);

        return;
    }
}
