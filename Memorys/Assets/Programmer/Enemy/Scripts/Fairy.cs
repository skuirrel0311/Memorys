using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class Fairy : MonoBehaviour
{
    //警戒しているか？
    public bool IsWarning;
    bool oldIsSeePlayer;
    public bool canMagic = false;
    public bool isShout = false;

    //見失ったか？
    public bool IsLostTarget;
    Vector3 lostPosition;

    //警戒度(どの程度警戒しているか)
    //public float Alertness { get; private set; }
    public float Alertness;

    BehaviorTree m_tree;
    PlayerController player;
    Vector3 targetPosition;
    
    Coroutine colorCoroutine;
    Coroutine attackCoroutine;

    SoundWaveFinder playerFinder;

    Transform modelTransform;
    Timer shakeTimer;
    float startY = 0.0f;
    float targetY = 0.0f;

    [SerializeField]
    GameObject Waypoints = null;


    void Start()
    {
        m_tree = GetComponent<BehaviorTree>();
        player = PlayerController.I;
        playerFinder = player.GetComponent<SoundWaveFinder>();

        IsWarning = false;
        Alertness = 0.0f;
        IsLostTarget = false;
        lostPosition = Vector3.zero;
        shakeTimer = new Timer();
        shakeTimer.TimerStart(2.0f, true);
        modelTransform = transform.GetChild(1);

        GameObject plane = Waypoints;

        List<GameObject> wayPointList = new List<GameObject>();
        for (int i = 0; i < plane.transform.childCount; i++)
        {
            wayPointList.Add(plane.transform.GetChild(i).gameObject);
        }

        GetComponent<BehaviorTree>().GetVariable("WayPoints").SetValue(wayPointList);
    }

    void Update()
    {
        ShakePosition();

        if (playerFinder != null)
        {
            if(IsWarning == false && playerFinder.workingTimer.IsWorking && canMagic == false)
            {
                //todo:距離も考慮
                IsWarning = true;
                Alertness = 2.5f;
                lostPosition = player.transform.position;
                IsLostTarget = true;
            }
        }

        UpdateAlertness();

        SetTargetPosition();

        
    }

    void UpdateAlertness()
    {
        if (isShout) return;

        bool isSeePlayer = (bool)m_tree.GetVariable("IsSeePlayer").GetValue();
        if (isSeePlayer)
        {
            Alertness += Time.deltaTime * 2.0f;
            IsLostTarget = false;
        }
        else
        {
            Alertness -= Time.deltaTime;
        }

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        if (!IsWarning)
        {
            IsWarning = Alertness > 0.5f;
        }
        else
        {
            //見失ったか？
            if (isSeePlayer == false && oldIsSeePlayer == true)
            {
                IsLostTarget = true;
                lostPosition = player.transform.position;
            }

            if (canMagic == false)
            {
                canMagic = Alertness >= 2.9f;
            }
        }

        oldIsSeePlayer = isSeePlayer;
    }

    void ShakePosition()
    {
        shakeTimer.Update();

        if (shakeTimer.IsLimitTime)
        {
            shakeTimer.Reset();

            startY = modelTransform.localPosition.y;
            float range = 3.0f;
            if (targetY > 0)
                targetY = Random.Range(range * 0.3f, range);
            else
                targetY = Random.Range(-range * 0.3f, 0);
        }

        modelTransform.localPosition = new Vector3(0, TkUtils.FloatLerp(startY, targetY, shakeTimer.Progress * shakeTimer.Progress), 0);
    }

    public void SetTargetPosition()
    {
        if (!IsLostTarget)
            m_tree.GetVariable("TargetPosition").SetValue(player.transform.position);
        else
            m_tree.GetVariable("TargetPosition").SetValue(lostPosition);
    }

    //地形変化を激しくする
    public void Magic(float changeTime)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(ViolentlyTransition(changeTime));
        isShout = true;
        IsWarning = false;
        Alertness = 0.0f;

        //todo:↑のコルーチンが終わるまで音が変わる
    }

    IEnumerator ViolentlyTransition(float changeTime)
    {
        float intervalTime = GameManager.I.transitionInterval;
        GameManager.I.SetIntervalTime(intervalTime * 0.02f);
        ChangeLight(Color.red);

        yield return new WaitForSeconds(changeTime);

        GameManager.I.SetIntervalTime(intervalTime);
        isShout = false;
    }

    public void ChangeLight(Color targetColor)
    {
        if(colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = StartCoroutine(SetLight(targetColor, 1.0f));
    }

    public void ChangeLight(float r, float g, float b, float a)
    {
        ChangeLight(new Color(r, g, b, a));
    }

    IEnumerator SetLight(Color targetLightColor, float time)
    {
        float t = 0.0f;
        Light spotLight = transform.GetChild(0).GetComponent<Light>();
        Color currentColor;
        Color startColor = spotLight.color;

        Renderer r = spotLight.transform.GetChild(0).GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        while (true)
        {
            currentColor = Color.Lerp(startColor, targetLightColor, t / time);
            spotLight.color = currentColor;

            currentColor.a = 0.025f;

            block.SetColor("_TintColor", currentColor);
            r.SetPropertyBlock(block);
            t += Time.deltaTime;

            if (t > time) break;
            yield return null;
        }
    }

    FloorTransition GetPlayerUnderFloor()
    {
        Collider[] cols = Physics.OverlapSphere(player.transform.position + Vector3.down, 1.0f);

        for (int i = 0; i < cols.Length; i++)
        {
            FloorTransition temp = cols[i].GetComponent<FloorTransition>();

            if (temp != null) return temp;
        }

        return null;
    }
}
