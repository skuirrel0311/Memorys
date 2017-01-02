using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

//決められた最小の角度と最大の角度の間をさまよいます
public class LookAround : Action
{
    public SharedFloat minRotateY;
    public SharedFloat maxRotateY;

    public SharedFloat rotateSpeed;
    
    float currentRotateY;

    private enum ComparisonOperation { Greater, Less}
    ComparisonOperation operation;

    public override void OnStart()
    {
        currentRotateY = transform.localEulerAngles.y;
        if (currentRotateY > maxRotateY.Value) operation = ComparisonOperation.Greater;
        if (currentRotateY < minRotateY.Value) operation = ComparisonOperation.Less;
    }

    public override TaskStatus OnUpdate()
    {
        //行きは増える帰りは減る
        if (operation == ComparisonOperation.Greater)
            currentRotateY -= Time.deltaTime * rotateSpeed.Value;
        else
            currentRotateY += Time.deltaTime * rotateSpeed.Value;

        if (currentRotateY > maxRotateY.Value) operation = ComparisonOperation.Greater;
        if (currentRotateY < minRotateY.Value) operation = ComparisonOperation.Less;

        transform.localRotation = Quaternion.Euler(0,currentRotateY,0);
        return TaskStatus.Running;
    }
}
