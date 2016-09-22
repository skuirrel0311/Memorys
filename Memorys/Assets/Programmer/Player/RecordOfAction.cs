using UnityEngine;
using UnityEngine.UI;

public class RecordOfAction : MonoBehaviour
{
    [SerializeField]
    float recordLength = 2;

    StorageOfAction[] actions = new StorageOfAction[3];
    Animator animator;

    public bool IsRecording;
    Timer recordTimer;

    public bool IsPlaying;
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
        IsRecording = false;
        recordTimer = new Timer();
        selectMemoryIndex = 0;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) RecordStart();
        if (Input.GetKeyDown(KeyCode.P)) ActionStart();

        if (IsRecording) Recording();
        if (IsPlaying) PlayingAction();

        ChangeMemory();
    }

    void RecordStart()
    {
        if (IsRecording) return;
        IsRecording = true;
        recordTimer.TimerStart(recordLength);
        actions[selectMemoryIndex].RecordStart();
    }

    void Recording()
    {
        recordTimer.Update();

        actions[selectMemoryIndex].Recording();

        if (recordTimer.IsLimitTime)
        {
            recordTimer.Stop(true);
            IsRecording = false;
            actions[selectMemoryIndex].StopRecord();
        }
    }

    void ActionStart()
    {
        if (IsAllPlayed()) return;
        IsPlaying = true;
        playTime = 0;
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
        playTime++;

        if (actions[playMemoryIndex].actionLog.Count <= playTime) NextAction();
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
                IsPlaying = false;
                return;
            }

            if (actions[playMemoryIndex].IsRecorded) break;
        }
        playTime = 0;
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
        if (IsRecording) GUI.TextField(new Rect(10, 10, 100, 30), "recording");

        if (IsPlaying) GUI.TextField(new Rect(10, 10, 100, 30), "playing");
    }

    void ChangeMemory()
    {
        //左右キーで保存するメモリ領域を変更する。
        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectMemoryIndex--;
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectMemoryIndex++;

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
