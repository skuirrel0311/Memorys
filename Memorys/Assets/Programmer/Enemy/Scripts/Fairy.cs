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
    bool IsLostTarget;
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
    }

    void Update()
    {
        ShakePosition();
        //警戒度がたまりやすくする
        bool isSeePlayer = (bool)m_tree.GetVariable("IsSeePlayer").GetValue();
        if (isSeePlayer)
        {
            Alertness += Time.deltaTime;
            IsLostTarget = false;
        }

        if (IsLostTarget)
        {
            Alertness -= Time.deltaTime;
        }

        Alertness = Mathf.Clamp(Alertness, 0.0f, 3.0f);

        if (Alertness == 3.0f)
        {
            canMagic = true;
        }
        IsWarning = Alertness > 1.5f;

        if (playerFinder != null)
        {
            if (playerFinder.workingTimer.IsWorking)
            {
                //todo:距離も考慮
                IsWarning = true;
                SetTargetPosition();
            }
        }

        //見失ったか？
        if (IsWarning)
        {
            if (isSeePlayer == false && oldIsSeePlayer == true)
            {
                IsLostTarget = true;
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
        if (IsLostTarget)
            m_tree.GetVariable("TargetPosition").SetValue(player.transform.position);
        else
            m_tree.GetVariable("TargetPosition").SetValue(lostPosition);
    }

    public IEnumerator RandomWalk()
    {
        Vector3 movement = Vector3.zero;
        float t = 0.0f;
        while (IsWarning)
        {
            t = 0.0f;
            //todo:ダサいので必修正
            float range = 0.01f;
            movement.x = TkUtils.FloatLerp(-range, range, Random.Range(0.0f, 1.0f));
            movement.y = TkUtils.FloatLerp(-range, range, Random.Range(0.0f, 1.0f));
            movement.z = TkUtils.FloatLerp(-range, range, Random.Range(0.0f, 1.0f));
            movement.Normalize();
            while (true)
            {
                t += Time.deltaTime;
                transform.Translate(movement * 0.1f);
                if (t > 0.2f) break;
                yield return null;
            }
        }
        yield return null;
    }

    //地形変化を激しくする
    public void Magic(float changeTime)
    {
        if (coroutineList.Count != 0)
        {
            for (int i = 0; i < coroutineList.Count; i++) StopCoroutine(coroutineList[i]);
        }
        coroutineList.Add(StartCoroutine(ViolentlyTransition(changeTime)));

        coroutineList.Add(StartCoroutine(RandomWalk()));
        canMagic = false;
        //todo:エフェクト
    }

    IEnumerator ViolentlyTransition(float changeTime)
    {
        float intervalTime = GameManager.I.transitionInterval;
        GameManager.I.SetIntervalTime(intervalTime * 0.02f);
        coroutineList.Add(StartCoroutine(SetLight(Color.red, 1.0f)));

        yield return new WaitForSeconds(changeTime);

        GameManager.I.SetIntervalTime(intervalTime);
        coroutineList.Add(StartCoroutine(SetLight(Color.white, 1.0f)));

        for (int i = coroutineList.Count - 1; i >= 0; i--)
        {
            coroutineList[i] = null;
            coroutineList.Remove(coroutineList[i]);
        }
        IsWarning = false;
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
