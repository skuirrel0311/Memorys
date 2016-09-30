using UnityEngine;
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

    public RecordState m_RecordState;

    [SerializeField]
    float recordLength = 2;

    //記録しているアクションが切り替わったときの演出
    [SerializeField]
    ParticleSystem m_ChangeParticle;

    [SerializeField]
    List<MonoBehaviour> PlayImageEffects;

    StorageOfAction[] actions = new StorageOfAction[3];
    Animator animator;

    Timer recordTimer;

    int playTime;

    public int selectMemoryIndex;
    public int playMemoryIndex;

    [SerializeField]
    Text selectText = null;
    [SerializeField]
    Text gaugeText = null;

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
        if (Input.GetButtonDown("Fire3")) RecordStart();
        if (Input.GetButtonDown("Jump")) ActionStart();

        switch (m_RecordState)
        {
            case RecordState.PLAY:
                actions[playMemoryIndex].AnalysisBehaior(playTime);
                break;
        }

    }

    void RecordStart()
    {

        if (m_RecordState == RecordState.RECORD) return;

        m_RecordState = RecordState.RECORD;

        for(int  i = actions.Length-1; i>=0;i--)
        {
            if (i != 0)
            {
                actions[i] = actions[i - 1];
            }
            else
            {
                actions[i] = new StorageOfAction(gameObject,animator);
            }
        }

        recordTimer.TimerStart(recordLength);
        //選択されている要素にレコードを開始
        actions[selectMemoryIndex].RecordStart();
    }

    void Recording()
    {
        recordTimer.Update();

        actions[selectMemoryIndex].Recording();
        if (recordTimer.IsLimitTime)
        {
            recordTimer.Stop(true);
            m_RecordState = RecordState.STAY;
            actions[selectMemoryIndex].StopRecord();
        }
    }
    
    void ActionStart()
    {
        if (IsAllPlayed()) return;
        if (m_RecordState == RecordState.PLAY) return;
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
