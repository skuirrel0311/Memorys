using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class EnemyStopItem : PopUpMessage
{
    bool isPush = false;
    TotemPaul[] enemies;

    public override void Start()
    {
        Renderer r = GetComponent<Renderer>();
        r.material.EnableKeyword("_EMISSION");
        //r.material.SetColor("_EmissionColor", new Color(0.3265f, 1.306f, 0.4548484f));

        base.Start();
    }

    //アイテムを使った時に停止する敵をセットする
    public void SetPairEnemy(TotemPaul[] enemies)
    {
        this.enemies = enemies;
    }

    public void SetPairEnemy(TotemPaul enemy)
    {
        enemies = new TotemPaul[1];
        enemies[0] = enemy;
    }

    public override void DrawMessage()
    {
        if (isPush) return;
        if (Time.timeScale == 0.0f) return;
        if (IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            NotificationSystem.I.Indication("石像が" + (enemies.Length).ToString() + "体、活動を止めた……",35);
            AkSoundEngine.PostEvent("Get_Switch", gameObject);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].Dead();
                enemies[i].GetComponent<BehaviorTree>().DisableBehavior(true);
            }
            isPush = true;
            IsViewMessage = false;

            Renderer r = GetComponent<Renderer>();
            r.material.DisableKeyword("_EMISSION");
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
