using UnityEngine;
using System.Collections;

public class BreakMessage : PopUpMessage
{
    public override void DrawMessage()
    {
        //todo:押したらどうのこうの
        base.DrawMessage();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "Player") return;

        IsViewMessage = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag != "Player") return;

        IsViewMessage = false;
    }
}
