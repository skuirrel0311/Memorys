using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceMessage : PopUpMessage
{
    public float distance = 0.0f;

    public override void DrawMessage()
    {
        if(IsViewMessage)
        {
            origin = transform.position;
            offset = transform.rotation * Vector3.left;
            
            messagePrefab.GetComponentInChildren<Text>().text = distance.ToString("F2") + "m";
        }
        base.DrawMessage();
    }

}
