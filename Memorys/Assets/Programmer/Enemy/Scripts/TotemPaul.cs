using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class TotemPaul : MonoBehaviour
{
    [SerializeField]
    protected GameObject shotEffect = null;
    protected ParticleSystem chargeEffect;
    protected ParticleSystem playerHitEffect;
    protected ParticleSystem objectHitEffect;

    protected BehaviorTree m_tree;
    protected Transform playerNeck;
    PlayerController playerController;
    protected Vector3 targetPosition;

    protected bool IsAttacking;
    public bool IsWarning { get; private set; }
    //警戒度
    public float Alertness = 0.0f;

    //動いている時のマテリアル
    [SerializeField]
    private Material activeMat = null;

    [SerializeField]
    bool IsAwakeActive = false;

    public bool IsStop = false;
    public bool IsDead = false;

    public virtual void Start()
    {
        chargeEffect = transform.FindChild("Charge/ball").GetComponent<ParticleSystem>();
        playerNeck = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_tree = GetComponent<BehaviorTree>();
        IsAttacking = false;
        IsWarning = false;
        
        if (!IsAwakeActive)
        {
            m_tree.enabled = false;
            //ライトをoffにする
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            QuickStartUp();
        }
    }

    public virtual void Update()
    {
        //起動していなかったら
        if (!m_tree.enabled) return;

        if (IsDead) return;

        if(IsStop)
        {
            if(!GameManager.I.IsPlayStop)
            {
                m_tree.EnableBehavior();
                IsStop = false;
            }
        }
        else if(GameManager.I.IsPlayStop)
        {
            m_tree.DisableBehavior(true);
            IsStop = true;
            return;
        }

        if ((bool)m_tree.GetVariable("IsSeePlayer").GetValue())
            Alertness += Time.deltaTime * 3;
        else
            Alertness -= Time.deltaTime;

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        if (IsWarning)
        {
            //一度警戒すると解けにくくなる
            IsWarning = Alertness > 0.5f;
        }
        else
        {
            //初回は警戒するまでに時間がかかる
            IsWarning = Alertness > 1.5f;
        }

        if (Alertness > 0.5f)
        {
            RotateTowards(playerNeck.position);
        }
    }

    protected virtual Vector3 GetTargetPosition()
    {
        if (IsWarning)
        {
            GameObject temp = (GameObject)m_tree.GetVariable("Player").GetValue();
            if (Random.Range(1, 100) < 33 && temp != null)
            {
                return temp.transform.position;
            }

            //警戒している時はターゲットの位置を更新する
            Vector3 movement = playerController.movement;
            movement.y = Random.Range(-0.15f, 0.0f);
            //何フレーム先の座標を読むか *（どのくらいの時間で弾が到着するのか）
            float futureRate = Random.Range(0.85f,0.95f) * (transform.position - playerNeck.position).magnitude;
            targetPosition = playerNeck.position + (movement * futureRate);
        }
        return targetPosition;
    }

    //戻り値は「向き終わったか」
    protected bool RotateTowards()
    {
        Vector3 vec = targetPosition - transform.position;
        vec.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.5f);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            return true;
        return false;
    }

    protected void RotateTowards(Vector3 targetPosition)
    {
        Vector3 vec = targetPosition - transform.position;
        vec.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.5f);
    }

    protected virtual void Shot(Vector3 target)
    {
        GameObject g = Instantiate(shotEffect, chargeEffect.transform.position, chargeEffect.transform.rotation);
        Vector3 velocity = target - chargeEffect.transform.position;
        g.GetComponent<Bullet>().SetUp(velocity.normalized,playerHitEffect, objectHitEffect, gameObject);
        g.transform.LookAt(target);

        Destroy(g, 10.0f);
    }

    //起動
    public void StartUp()
    {
        if (IsAwakeActive) return;
        QuickStartUp();
    }

    public void QuickStartUp()
    {
        m_tree.enabled = true;
        
        Light light = transform.GetChild(1).GetComponent<Light>();
        light.gameObject.SetActive(true);
        light.spotAngle = (float)m_tree.GetVariable("ViewAngle").GetValue();
        light.range = (float)m_tree.GetVariable("ViewDistance").GetValue();
        float angleY = (float)m_tree.GetVariable("ViewAngleY").GetValue() - light.spotAngle;
        angleY = (angleY * 0.5f) - (30.0f * (1 - (angleY / 90.0f)));
        light.transform.localRotation = Quaternion.Euler(new Vector3(angleY, 0, 0));

        Material[] mats = GetComponent<Renderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = activeMat;
        }

        GetComponent<Renderer>().materials = mats;
    }

    public void Dead()
    {
        IsDead = true;
        IsWarning = false;
        Alertness = 0.0f;
        transform.GetChild(1).gameObject.SetActive(false);

        //todo:緩やかにor演出
        Material[] mats = GetComponent<Renderer>().materials;
        for(int i = 0;i < mats.Length;i++)
        {
            mats[i].DisableKeyword("_EMISSION");
        }
    }
}
