using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class Fairy : MonoBehaviour
{
    //警戒しているか？
    public bool IsWarning { get; private set; }
    bool oldIsSeePlayer;

    //見失ったか？
    bool IsLostTarget;
    Vector3 lostPosition;

    //警戒度(どの程度警戒しているか)
    //public float Alertness { get; private set; }
    public float Alertness;

    BehaviorTree m_tree;
    PlayerController player;

    [SerializeField]
    GameObject magicEffect = null;
    Coroutine coroutine = null;

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
        shakeTimer.TimerStart(2.0f,true);
        modelTransform = transform.GetChild(1);
    }

    void Update()
    {
        ShakePosition();
        //警戒度がたまりやすくする
        bool isSeePlayer = (bool)m_tree.GetVariable("IsSeePlayer").GetValue();
        if (isSeePlayer)
        {
            Alertness += Time.deltaTime * 3.0f;
            IsLostTarget = false;
        }

        Alertness = Mathf.Min(Alertness, 3.0f);

        IsWarning = Alertness > 1.5f;

        if (playerFinder != null)
        {
            if (playerFinder.workingTimer.IsWorking)
            {
                //todo:距離も考慮
                IsWarning = true;
            }
        }

        //見失ったか？
        if (IsWarning)
        {
            if (isSeePlayer == false && oldIsSeePlayer == true)
            {
                IsLostTarget = true;
                lostPosition = player.transform.position;
                Alertness = 0.0f;
            }
        }

        oldIsSeePlayer = isSeePlayer;
    }

    void ShakePosition()
    {
        shakeTimer.Update();

        if(shakeTimer.IsLimitTime)
        {
            shakeTimer.Reset();

            startY = modelTransform.localPosition.y;
            float range = 3.0f;
            if (targetY > 0)
                targetY = Random.Range(range * 0.3f, range);
            else
                targetY = Random.Range(-range * 0.3f, 0);
        }
        
        modelTransform.localPosition = new Vector3(0, MovementUtility.FloatLerp(startY, targetY, shakeTimer.Progress * shakeTimer.Progress), 0);
    }

    public void GetTragetPosition()
    {
        if (IsLostTarget)
            m_tree.GetVariable("TargetPosition").SetValue(player.transform.position);
        else
            m_tree.GetVariable("TargetPosition").SetValue(lostPosition);
    }

    //地形変化を激しくする
    public void Magic(float changeTime)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ViolentlyTransition(changeTime));
        //todo:エフェクト
    }

    IEnumerator ViolentlyTransition(float changeTime)
    {
        float intervalTime = GameManager.I.transitionInterval;
        GameManager.I.SetIntervalTime(intervalTime * 0.05f);

        yield return new WaitForSeconds(changeTime);

        GameManager.I.SetIntervalTime(intervalTime);
        coroutine = null;
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
