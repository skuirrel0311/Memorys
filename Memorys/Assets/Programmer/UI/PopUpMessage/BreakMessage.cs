using UnityEngine;
using BehaviorDesigner.Runtime;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;

    //何回ボタンを押せばいいか
    [SerializeField]
    int maxPushCount = 5;
    int pushCount = 0;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
    }

    public override void DrawMessage()
    {
        //todo:押したらどうのこうの
        messagePrefab.fillAmount = (float)pushCount / maxPushCount;
        if(IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            pushCount++;
        }

        if (pushCount > maxPushCount)
        {
            //エフェクト
            GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);
            GameManager.I.PushSwitch();

            IsViewMessage = false;
            pushCount = 0;
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
