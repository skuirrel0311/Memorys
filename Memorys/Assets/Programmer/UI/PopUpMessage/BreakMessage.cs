using UnityEngine;
using BehaviorDesigner.Runtime;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;
    BehaviorTree[] enemies;
    static int count = 1;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
        count = 1;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void DrawMessage()
    {
        //todo:押したらどうのこうの
        if (IsViewMessage&&(MyInputManager.GetButtonDown(MyInputManager.Button.A)||Input.GetKeyDown(KeyCode.Delete)))
        {
            GameManager.I.DestroyCancel();
            //エフェクト
            GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);

            for (int i = 0; i < GameManager.I.enemies.Length; i++)
            {
                //中心のトーテムポール
                if (GameManager.I.enemies[i].gameObject.name == "TotemPaul (" + count.ToString() + ")")
                {
                    GameManager.I.enemies[i].GetComponent<TotemPaul>().StartUp();
                }
            }

            count++;
            IsViewMessage = false;
            Destroy(gameObject);
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
