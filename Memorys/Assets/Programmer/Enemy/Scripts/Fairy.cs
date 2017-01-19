using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class Fairy : MonoBehaviour
{
    //警戒しているか？
    public bool IsWarning { get; private set; }
    bool oldIsSeePlayer;
    public bool canMagic = false;

    //見失ったか？
    public bool IsLostTarget;
    Vector3 lostPosition;

    //警戒度(どの程度警戒しているか)
    //public float Alertness { get; private set; }
    public float Alertness;

    BehaviorTree m_tree;
    PlayerController player;
    Vector3 targetPosition;

    List<Coroutine> coroutineList = new List<Coroutine>();

    SoundWaveFinder playerFinder;

    Transform modelTransform;
    Timer shakeTimer;
    float startY = 0.0f;
    float targetY = 0.0f;

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

        GameObject plane = GameObject.Find("Plane");

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
            if (playerFinder.workingTimer.IsWorking)
            {
                //todo:距離も考慮
                IsWarning = true;
            }
        }

        //警戒度がたまりやすくする
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

            if(IsWarning)
            {
                coroutineList.Add(StartCoroutine(SetLight(Color.yellow, 1.0f)));
            }
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
            if (Alertness <= 0.1f && !canMagic)
            {
                //見失った
                IsWarning = false;
                coroutineList.Add(StartCoroutine(SetLight(Color.white, 1.0f)));
            }
        }
        SetTargetPosition();
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
        if (coroutineList.Count != 0)
        {
            for (int i = 0; i < coroutineList.Count; i++) StopCoroutine(coroutineList[i]);
        }
        coroutineList.Add(StartCoroutine(ViolentlyTransition(changeTime)));
    }

    IEnumerator ViolentlyTransition(float changeTime)
    {
        float intervalTime = GameManager.I.transitionInterval;
        GameManager.I.SetIntervalTime(intervalTime * 0.02f);
        coroutineList.Add(StartCoroutine(SetLight(Color.red, 1.0f)));

        yield return new WaitForSeconds(changeTime);

        GameManager.I.SetIntervalTime(intervalTime);
        coroutineList.Add(StartCoroutine(SetLight(Color.white, 1.0f)));

        IsWarning = false;
        canMagic = false;
        Alertness = 0.0f;
        for (int i = coroutineList.Count - 1; i >= 0; i--)
        {
            coroutineList[i] = null;
            coroutineList.Remove(coroutineList[i]);
        }

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
