﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public  enum RecordState
{
    RECORD,
    PLAY,
    STAY
}

public class RecordOfAction : MonoBehaviour
{
    public static RecordOfAction I;
    public RecordState m_RecordState;
    public int selectMemoryIndex;
    public int playMemoryIndex;


    [SerializeField]
    private float recordLength = 2;
    //記録しているアクションが切り替わったときの演出
    [SerializeField]
    private ParticleSystem m_ChangeParticle;
    [SerializeField]
    private List<MonoBehaviour> PlayImageEffects;
    [SerializeField]
    private Text selectText = null;
    [SerializeField]
    private Text gaugeText = null;

    private StorageOfAction[] actions = new StorageOfAction[3];
    private Animator animator;
    private Timer recordTimer;
    private int playTime;


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = new StorageOfAction(gameObject, animator);
        }
        m_RecordState = RecordState.STAY;
        recordTimer = new Timer();
        selectMemoryIndex = 0;
        I = this;
    }

    void FixedUpdate()
    {
        switch (m_RecordState)
        {
            case RecordState.RECORD:
                Recording();
                break;
            case RecordState.PLAY:
                PlayingAction();
                break;
        }

        ChangeMemory();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire4")) RecordStart();
        if (Input.GetButtonDown("Fire5")) ActionStart();
        if(Input.GetButtonDown("Fire2"))
        {
            ActionReset();
        }
        switch (m_RecordState)
        {
            case RecordState.PLAY:
                actions[playMemoryIndex].AnalysisBehaior(playTime);
                break;
        }

    }

    void ActionReset()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = new StorageOfAction(gameObject, animator);
        }
    }

    void RecordStart()
    {

        if (m_RecordState != RecordState.STAY) return;

        m_RecordState = RecordState.RECORD;
        int count=0;
        for(int i=0;i<actions.Length;i++)
        {
            if (actions[i].IsRecorded)
            {
                count++;
            }
        }
        if (count == actions.Length)
        {
            //一つずつずらす
            for (int i = 0; i < actions.Length; i++)
            {
                if (i != actions.Length - 1)
                {
                    actions[i] = actions[i + 1];
                }
                else
                {
                    actions[0] = new StorageOfAction(gameObject, animator);
                    //選択されている要素にレコードを開始
                    recordTimer.TimerStart(recordLength);
                    actions[0].RecordStart();
                }
            }
        }
        else
        {
            //選択されている要素にレコードを開始
            actions[count] = new StorageOfAction(gameObject, animator);
            recordTimer.TimerStart(recordLength);
            actions[count].RecordStart();
            Debug.Log("Count:"+count);
        }

    }

    void Recording()
    {
        int count = 0;
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i].IsRecorded)
            {
                count++;
            }
        }
        recordTimer.Update();
        actions[count].Recording();
        if (recordTimer.IsLimitTime)
        {
            recordTimer.Stop(true);
            m_RecordState = RecordState.STAY;
            actions[count].StopRecord();
        }
    }
    
    void ActionStart()
    {
        if (IsAllPlayed()) return;
        if (m_RecordState != RecordState.STAY) return;
        m_RecordState = RecordState.PLAY;
        EnablePlayImageEffects(true);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        playTime = 0;
        //データが保存されているものはスタートの状態をセット
        for (int i = 0; i < 3; i++)
        {
            if (!actions[i].IsRecorded) continue;
            playMemoryIndex = i;
            actions[i].StartAction();
            break;
        }
    }

    void PlayingAction()
    {
        actions[playMemoryIndex].PlayingAction(playTime);
        playTime+=2;

        if (actions[playMemoryIndex].actionLog.Count <= playTime)
        {
            NextAction();
        }
    }

    void EnablePlayImageEffects(bool isEnabled)
    {
        for(int i = 0;i< PlayImageEffects.Count;i++)
        {
            PlayImageEffects[i].enabled = isEnabled;
        }
    }

    void NextAction()
    {
        //現在のアクションは止める
        actions[playMemoryIndex].StopAction();

        //次のアクション
        while (true)
        {
            playMemoryIndex++;

            if (playMemoryIndex >= actions.Length)
            {
                //範囲外に出たら終了
                m_RecordState = RecordState.STAY;
                EnablePlayImageEffects(false);
                return;
            }

            if (actions[playMemoryIndex].IsRecorded) break;
        }
        playTime = 0;
        m_ChangeParticle.transform.localPosition = transform.position;
        m_ChangeParticle.Play();
        actions[playMemoryIndex].StartAction();
    }

    bool IsAllPlayed()
    {
        //記録されていなかったらプレイはし終わっている
        foreach (StorageOfAction s in actions)
        {
            if (s.IsRecorded) return false;
        }

        return true;
    }

    void OnGUI()
    {
        if (m_RecordState == RecordState.RECORD) GUI.TextField(new Rect(10, 10, 100, 30), "recording");

        if (m_RecordState == RecordState.PLAY) GUI.TextField(new Rect(10, 10, 100, 30), "playing");
    }

    void ChangeMemory()
    {
        selectMemoryIndex = Mathf.Clamp(selectMemoryIndex, 0, actions.Length - 1);
        char[] str1 = new char[3];
        char[] str2 = new char[3];

        for (int i = 0; i < actions.Length; i++)
        {
            if (i != selectMemoryIndex) str1[i] = '　';
            else str1[i] = '↓';

            if (actions[i].IsRecorded) str2[i] = '●';
            else str2[i] = '○';
        }

        selectText.text = new string(str1);

        gaugeText.text = new string(str2);
    }

}
