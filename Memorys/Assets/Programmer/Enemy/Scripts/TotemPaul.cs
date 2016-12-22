using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class TotemPaul : MonoBehaviour
{
    [SerializeField]
    protected GameObject shotEffect = null;
    protected ParticleSystem chargeEffect;

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

    public virtual void Start()
    {
        chargeEffect = transform.FindChild("Charge/ball").GetComponent<ParticleSystem>();
        playerNeck = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        IsAttacking = false;
        IsWarning = false;


        if (!IsAwakeActive)
        {
            GetComponent<BehaviorTree>().enabled = false;
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
        if (!GetComponent<BehaviorTree>().enabled) return;

        if ((bool)GetComponent<BehaviorTree>().GetVariable("IsSeePlayer").GetValue())
            Alertness += Time.deltaTime * 3;
        else
            Alertness -= Time.deltaTime;

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        if (IsWarning)
        {
            IsWarning = Alertness > 0.5f;
        }
        else
        {
            IsWarning = Alertness > 2.5f;
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
            //警戒している時はターゲットの位置を更新する
            Vector3 movement = playerNeck.position - playerController.oldPosition;
            movement.y = 0.0f;

            //なんフレーム先の座標を読むか
            float futureRate = 0.9f * (transform.position - playerNeck.position).magnitude;
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
        g.GetComponent<Bullet>().SetUp(velocity.normalized);

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
        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);

        Material[] mats = GetComponent<Renderer>().materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = activeMat;
        }

        GetComponent<Renderer>().materials = mats;
    }
}
