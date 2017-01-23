using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public class TotemPaul : MonoBehaviour
{
    [SerializeField]
    protected GameObject shotEffect = null;
    protected ParticleSystem chargeEffect;
    protected ParticleSystem playerHitEffect;
    protected ParticleSystem objectHitEffect;

    protected WaitForSeconds intervalWait;
    [SerializeField]
    protected float intervalTime = 1.0f;

    protected BehaviorTree m_tree;
    protected Transform playerNeck;
    PlayerController playerController;
    protected Vector3 targetPosition;

    public bool IsAttacking;
    Coroutine attackCoroutine;
    public bool IsWarning { get; private set; }
    //警戒度
    public float Alertness = 0.0f;

    //動いている時のマテリアル
    [SerializeField]
    private Texture activeTex = null;
    [SerializeField]
    private Texture emissionMap = null;
    [SerializeField]
    private Texture normalMap = null;
    protected List<Coroutine> activateCoroutineList = new List<Coroutine>();

    [SerializeField]
    bool IsAwakeActive = false;

    public bool IsStop = false;
    public bool IsDead = false;
    public bool IsCameraEffect = false;

    public virtual void Start()
    {
        chargeEffect = transform.FindChild("Charge/ball").GetComponent<ParticleSystem>();
        playerNeck = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Hips/Spine/Spine1/Spine2/Neck");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_tree = GetComponent<BehaviorTree>();
        intervalWait = new WaitForSeconds(intervalTime);
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

        if (IsStop)
        {
            if (!GameManager.I.IsPlayStop)
            {
                m_tree.EnableBehavior();
                IsStop = false;
            }
        }
        else if (GameManager.I.IsPlayStop)
        {
            m_tree.DisableBehavior(true);
            IsStop = true;
            return;
        }

        if ((bool)m_tree.GetVariable("IsSeePlayer").GetValue())
        {
            Alertness += Time.deltaTime * 3.0f;
        }
        else
        {
            float angleY = TkUtils.GetAngleY(chargeEffect.transform.position, targetPosition);
            if (angleY > 70.0f)
                Alertness -= Time.deltaTime * 3.0f;
            Alertness -= Time.deltaTime;
        }

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        if (IsWarning)
        {
            //一度警戒すると解けにくくなる
            IsWarning = Alertness > 1.0f;
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

            Vector3 movement = playerController.movement;

            if (IsPlayerNotMove())
            {
                //動いていなかったらその場から適当にばらしたところをターゲットにする
                movement = Vector3.Cross(playerNeck.position - chargeEffect.transform.position, Vector3.up);
                movement *= Random.Range(-0.2f, 0.2f);
            }
            else
            {
                movement.y = Random.Range(-0.3f, 0.2f);
                //何フレーム先の座標を読むか *（どのくらいの時間で弾が到着するのか）
                float futureRate = Random.Range(0.70f, 1.30f) * (transform.position - playerNeck.position).magnitude;
                movement *= futureRate;
            }
            targetPosition = playerNeck.position + (movement);
        }
        return targetPosition;
    }

    //戻り値は「向き終わったか」
    protected bool RotateTowards()
    {
        Vector3 vec = targetPosition - transform.position;
        vec.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 30.0f * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            return true;
        return false;
    }

    protected void RotateTowards(Vector3 targetPosition)
    {
        Vector3 vec = targetPosition - transform.position;
        vec.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 30.0f * Time.deltaTime);
    }

    public void Attack()
    {
        if (IsAttacking) return;

        targetPosition = playerNeck.position;
        attackCoroutine = StartCoroutine(Attacking());
    }

    protected virtual IEnumerator Attacking()
    {
        yield return null;
    }

    protected virtual void Shot(Vector3 target)
    {
        GameObject g = Instantiate(shotEffect, chargeEffect.transform.position, chargeEffect.transform.rotation);
        Vector3 velocity = target - chargeEffect.transform.position;
        g.GetComponent<Bullet>().SetUp(velocity.normalized, playerHitEffect, objectHitEffect, gameObject);
        g.transform.LookAt(target);

        Destroy(g, 10.0f);
    }

    //起動
    public void StartUp()
    {
        if (IsAwakeActive) return;
        if(IsCameraEffect)
        {
            playerController.currentState = PlayerState.Idle;
            CameraManager.I.CameraChange(2,1.5f);
        }
        activateCoroutineList.Add(StartCoroutine("QuickStartUp"));
    }

    public IEnumerator QuickStartUp()
    {
        GetComponent<Renderer>().materials = GetActiveMaterial();
        Coroutine coroutine = StartCoroutine(SetColor(true));
        activateCoroutineList.Add(coroutine);
        //カラーの適用を待つ
        yield return coroutine;

        if (IsDead) yield break;
        //BehaviorTreeを起動する
        m_tree.enabled = true;

        Light light = transform.GetChild(1).GetComponent<Light>();
        light.gameObject.SetActive(true);
        light.spotAngle = (float)m_tree.GetVariable("ViewAngle").GetValue();

        float scale = (light.spotAngle / 30.0f) * 60.0f * 1.125f;
        float angleY = (2.0f + (light.spotAngle / 30.0f)) * 15.0f;
        angleY = 90.0f - angleY;
        float range = (light.spotAngle / 30.0f) * 15;

        light.transform.GetChild(0).localScale = new Vector3(scale, 75.0f, scale);
        light.transform.localRotation = Quaternion.Euler(new Vector3(angleY, 0, 0));
        m_tree.GetVariable("ViewDistance").SetValue(range);
        StartCoroutine(StartUpLight());
    }

    protected Material[] GetActiveMaterial()
    {
        Material[] mats = GetComponent<Renderer>().materials;

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetTexture("_MainTex", activeTex);
            mats[i].SetTexture("_EmissionMap", emissionMap);
            mats[i].SetTexture("_BumpMap", normalMap);
            mats[i].SetTexture("_DetailAlbedoMap", null);
            mats[i].SetTexture("_DetailNormalMap", null);
        }
        return mats;
    }

    protected virtual IEnumerator SetColor(bool atStart = false)
    {
        Color startColor = Color.black;
        Color endColor = new Color(1.0f, 0.4411765f, 0.4411765f);
        Color overColor = new Color(3.0f, 1.32353f, 1.32353f);

        Coroutine coroutine = StartCoroutine(SetEmissionColor(startColor, overColor, 1.0f));
        if (atStart) activateCoroutineList.Add(coroutine);
        yield return coroutine;

        coroutine = StartCoroutine(SetEmissionColor(overColor, endColor, 1.5f));
        if (atStart) activateCoroutineList.Add(coroutine);
        yield return coroutine;
    }

    protected IEnumerator SetEmissionColor(Color startColor, Color endColor, float time)
    {
        Material[] mats = GetComponent<Renderer>().materials;
        Color currentColor;
        float t = 0.0f;

        while (true)
        {
            currentColor = Color.Lerp(startColor, endColor, t);
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].SetColor("_EmissionColor", currentColor);
            }

            t += Time.deltaTime;
            if (t > time) break;
            yield return null;
        }
    }

    IEnumerator StartUpLight()
    {
        Light light = transform.GetChild(1).GetComponent<Light>();
        Transform lightModel = light.transform.GetChild(0);
        float endLightRange = light.range;
        Vector3 endModelSize = lightModel.localScale;
        float t = 0.0f;
        float progress = 0.0f;

        while(true)
        {
            t += Time.deltaTime;
            progress = t / 0.3f;
            progress = progress * progress;

            light.range = TkUtils.FloatLerp(0.0f, endLightRange, progress);
            lightModel.localScale = Vector3.Lerp(Vector3.zero, endModelSize, progress);

            if (t > 0.3f) break;
            yield return null;
        }
    }

    public virtual void Dead()
    {
        if (IsAttacking)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            IsAttacking = false;
        }

        foreach (Coroutine coroutine in activateCoroutineList)
        {
            StopCoroutine(coroutine);
        }

        IsDead = true;
        IsWarning = false;
        Alertness = 0.0f;
        transform.GetChild(1).gameObject.SetActive(false);

        //todo:緩やかにor演出
        StartCoroutine(SetEmissionColor(GetComponent<Renderer>().materials[0].GetColor("_EmissionColor"), Color.black, 2.0f));
    }

    private bool IsPlayerNotMove()
    {
        Vector3 movement = PlayerController.I.movement;
        //基準
        float norm = 0.1f;

        if (movement.x < norm && movement.z < norm) return true;

        return false;
        
    }
}
