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

    Vector3 startPosition, underPosition;

    void Awake()
    {
        startPosition = transform.position;
    }

    public virtual void Start()
    {
        chargeEffect = transform.FindChild("Sphere/Charge/ball").GetComponent<ParticleSystem>();
        playerNeck = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        IsAttacking = false;
        IsWarning = false;

        GetComponent<BehaviorTree>().enabled = false;
        //ライトをoffにする
        transform.GetChild(1).gameObject.SetActive(false);

        startPosition = transform.position;
        underPosition = new Vector3(transform.position.x, -transform.position.y - 15.0f, transform.position.z);
    }

    public virtual void Update()
    {
        if (!GetComponent<BehaviorTree>().enabled) transform.position = underPosition;

        if ((bool)GetComponent<BehaviorTree>().GetVariable("IsSeePlayer").GetValue())
            Alertness += Time.deltaTime * 5;
        else
            Alertness -= Time.deltaTime;

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);
        IsWarning = Alertness > 0.5f;
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
        StartCoroutine("Rising");

        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void QuickStartUp()
    {
        transform.position = startPosition;
        GetComponent<BehaviorTree>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
    }

    IEnumerator Rising()
    {
        float time = 0.0f;
        while (true)
        {
            time += Time.deltaTime * 0.5f;
            transform.position = Vector3.Lerp(underPosition, startPosition, time * time);
            if (transform.position.y > startPosition.y) break;
            yield return null;
        }
    }
}
