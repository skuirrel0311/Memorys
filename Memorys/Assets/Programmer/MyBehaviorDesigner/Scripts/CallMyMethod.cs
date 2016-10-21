using UnityEngine;
using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Reflection;

public class CallMyMethod : BehaviorDesigner.Runtime.Tasks.Action
{
    public SharedGameObject targetObject;
    public string componentName;
    public string methodName;
    public SharedVariable[] parameters;

    public override void OnAwake()
    {
        if(targetObject == null) targetObject = gameObject;
    }

    public override TaskStatus OnUpdate()
    {
        var type = TaskUtility.GetTypeWithinAssembly(componentName);
        if (type == null) return TaskStatus.Failure;

        var component = GetDefaultGameObject(targetObject.Value).GetComponent(type);
        if (component == null) return TaskStatus.Failure;

        FieldInfo fieldInfo = GetType().GetField("parameters");
        if (fieldInfo == null) return TaskStatus.Failure;

        object obj = fieldInfo.GetValue(this);

        component.SendMessage(methodName, obj);

        return TaskStatus.Success;
    }
}
