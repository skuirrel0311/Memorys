using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceMessage : AtScreenEdgeMessage
{
    public float distance = 0.0f;

    public override void DrawMessage()
    {
        base.DrawMessage();

        if (!IsViewMessage) return;
        if (targetTransform == null) return;

        //距離を小数点2桁まで表示する
        distance = (targetTransform.position -  transform.position).magnitude;
        messagePrefab.GetComponentInChildren<Text>().text = distance.ToString("F2") + "m";
    }
}
