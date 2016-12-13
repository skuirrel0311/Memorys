using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalObject : PopUpMessage
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Player") return;

        GameManager.I.m_GameEnd.GameClear();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag != "Player") return;
        origin = transform.position;
        IsViewMessage = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag != "Player") return;

        IsViewMessage = false;
    }
}
