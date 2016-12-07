using UnityEngine;
using BehaviorDesigner.Runtime;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;
    BehaviorTree[] enemies;
    GameObject player;
    static int count = 1;

    [SerializeField]
    int pushNum = 5;
    int pushCount = 0;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
        count = 1;
    }

    public override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void DrawMessage()
    {
        if (IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            //todo:担ぐ
            player.GetComponent<PlayerHasItem>().ToHaveItem(gameObject);
        }

        base.DrawMessage();
    }


    public void DraMessage()
    {
        //todo:押したらどうのこうの
        messagePrefab.fillAmount = (float)pushCount / pushNum;
        if(IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            pushCount++;
        }

        if (pushCount > pushNum)
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
