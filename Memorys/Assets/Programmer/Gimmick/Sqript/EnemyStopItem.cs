using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class EnemyStopItem : PopUpMessage
{
    bool isPush = false;
    TotemPaul enemy;

    //アイテムを使った時に停止する敵をセットする
    public void SetPairEnemy(TotemPaul enemy)
    {
        this.enemy = enemy;
    }

    public override void DrawMessage()
    {
        if (isPush) return;
        if (IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            enemy.Dead();
            enemy.GetComponent<BehaviorTree>().DisableBehavior(true);
            isPush = true;
            IsViewMessage = false;
        }

        base.DrawMessage();
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
