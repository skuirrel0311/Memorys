using UnityEngine;
using BehaviorDesigner.Runtime;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;
    BehaviorTree[] enemies;
    int count = 0;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
    }

    public override void Start()
    {
        base.Start();
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new BehaviorTree[tempArray.Length];
        for(int i = 0;i < tempArray.Length;i++)
        {
            enemies[i] = tempArray[i].GetComponent<BehaviorTree>();

            //中心のトーテムポール
            if(enemies[i].gameObject.name == "TotemPaul")
            {
                enemies[i].GetComponent<TotemPaul>().QuickStartUp();
                
                count = 1;
            }
        }
    }

    public override void DrawMessage()
    {
        //todo:押したらどうのこうの
        if (IsViewMessage&&(MyInputManager.GetButtonDown(MyInputManager.Button.A)||Input.GetKeyDown(KeyCode.Delete)))
        {
            GameManager.I.DestroyCancel();
            //エフェクト
            GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);

            for (int i = 0; i < enemies.Length; i++)
            {
                //中心のトーテムポール
                if (enemies[i].gameObject.name == "TotemPaul (" + count.ToString() + ")")
                {
                    enemies[i].GetComponent<TotemPaul>().StartUp();
                }
            }

            count++;
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
