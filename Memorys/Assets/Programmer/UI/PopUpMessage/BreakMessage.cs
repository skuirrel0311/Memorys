using UnityEngine;
using System.Collections;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
    }

    public override void DrawMessage()
    {
        //todo:押したらどうのこうの
        if (IsViewMessage&&(MyInputManager.GetButtonDown(MyInputManager.Button.A)||Input.GetKeyDown(KeyCode.Delete)))
        {
            GameManager.I.DestroyCancel();
            //エフェクト
            GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);
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
