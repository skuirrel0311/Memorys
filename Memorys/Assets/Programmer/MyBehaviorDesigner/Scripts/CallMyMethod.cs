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

        var parameterList = new List<object>();
        var parameterTypeList = new List<Type>();

        object obj = fieldInfo.GetValue(this);
        SharedVariable sharedVariable = null;
        foreach (object o in (Array)obj)
        {
            sharedVariable = o as SharedVariable;
            parameterList.Add(sharedVariable.GetValue());
            parameterTypeList.Add(sharedVariable.GetType().GetProperty("Value").PropertyType);
        }
        
        var methodInfo = component.GetType().GetMethod(methodName, parameterTypeList.ToArray());
        if (methodInfo == null) return TaskStatus.Failure;

        methodInfo.Invoke(component, parameterList.ToArray());

        return TaskStatus.Success;
    }
}
