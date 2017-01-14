using UnityEngine;
using BehaviorDesigner.Runtime;

public class BreakMessage : PopUpMessage
{
    GameObject m_Exposion;

    //何回ボタンを押せばいいか
    [SerializeField]
    int maxPushCount = 5;
    int pushCount = 0;

    public bool isPush = false;

    SoundWaveFinder finder;

    void Awake()
    {
        m_Exposion = Resources.Load("ExplosionMobile") as GameObject;
    }

    public override void Start()
    {
        finder = GameObject.FindGameObjectWithTag("Player").GetComponent<SoundWaveFinder>();
        isPush = false;
        base.Start();
    }

    public override void DrawMessage()
    {
        if (isPush) return;
        if (Time.timeScale == 0.0f) return;
        //todo:押したらどうのこうの
        messagePrefab.fillAmount = (float)pushCount / maxPushCount;
        if(IsViewMessage && (MyInputManager.GetButtonDown(MyInputManager.Button.X) || Input.GetKeyDown(KeyCode.Delete)))
        {
            pushCount++;
            AkSoundEngine.PostEvent("Get_Switch",gameObject);
        }

        if (pushCount > maxPushCount)
        {
            //エフェクト
           // GameObject.Instantiate(m_Exposion, transform.position, Quaternion.identity);
            GameManager.I.PushSwitch();
            isPush = true;
            Renderer r = GetComponent<Renderer>();
            r.material.EnableKeyword("_EMISSION");
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
            r.SetPropertyBlock(block);

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
