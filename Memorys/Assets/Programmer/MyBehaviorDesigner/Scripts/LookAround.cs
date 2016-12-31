using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

//決められた最小の角度と最大の角度の間をさまよいます
public class LookAround : Action
{
    public SharedFloat minRotateY;
    public SharedFloat maxRotateY;

    public SharedFloat rotateSpeed;

    bool isReturn = false;
    float currentRotateY;

    public override void OnStart()
    {
        currentRotateY = transform.localEulerAngles.y;
    }

    public override TaskStatus OnUpdate()
    {
        //行きは増える帰りは減る
        if (isReturn)
            currentRotateY -= Time.deltaTime * rotateSpeed.Value;
        else
            currentRotateY += Time.deltaTime * rotateSpeed.Value;

        if (currentRotateY > maxRotateY.Value || currentRotateY < minRotateY.Value)
        {
            isReturn = !isReturn;
        }

        currentRotateY = Mathf.Clamp(currentRotateY, minRotateY.Value, maxRotateY.Value);

        transform.localRotation = Quaternion.Euler(0,currentRotateY,0);
        return TaskStatus.Running;
    }
}
