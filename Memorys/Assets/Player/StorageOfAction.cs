using UnityEngine;
using System.Collections.Generic;

//todo:publicだらけなので要修正
//プレイヤーの行動を記憶しておくクラス
public class StorageOfAction
{
    GameObject player;
    Animator animator;
    Vector3 startPosition;
    Vector3 oldPosition;
    public List<Vector3> actionLog;

    //記録中か？
    public bool IsRecording;
    public bool IsPlaying;
    //記録されているか？
    public bool IsRecorded { get { return actionLog.Count != 0; } }

    /*コンストラクタ*/
    public StorageOfAction(GameObject player, Animator animator)
    {
        this.player = player;
        IsRecording = false;
        actionLog = new List<Vector3>();
        startPosition = Vector3.zero;
        oldPosition = Vector3.zero;
        this.animator = animator;
    }

    /*記録*/
    public void RecordStart()
    {
        IsRecording = true;
        startPosition = player.transform.position;
        oldPosition = player.transform.position;
        actionLog.Clear();
    }
    public void Recording()
    {
        //座標を記録する
        Vector3 movement = player.transform.position - oldPosition;
        movement.y = ToRoundDown(movement.y, 6);
        actionLog.Add(movement);
        oldPosition = player.transform.position;
        //Debug.Log("log[" + (actionLog.Count - 1) + "] = " + actionLog[actionLog.Count - 1]);
        Debug.Log("log[" + (actionLog.Count - 1) + "].y = " + actionLog[actionLog.Count - 1].y);
    }
    public void StopRecord()
    {
        IsRecording = false;
    }

    /*再生*/
    public void StartAction()
    {
        IsPlaying = true;
        player.GetComponent<Rigidbody>().useGravity = false;
    }
    public void PlayingAction(int playTime)
    {
        player.transform.position += actionLog[playTime];

        //アニメーション
        AnalysisBehaior(playTime);
        oldPosition = player.transform.position;
    }

    public void StopAction()
    {
        actionLog.Clear();
        player.GetComponent<Rigidbody>().useGravity = true;
        IsPlaying = false;
    }

    //行動を解析しアニメーションを再生させる
    void AnalysisBehaior(int playTime)
    {
        //Debug.Log(actionLog[playTime].y);

        /*移動していない*/
        if (actionLog[playTime] == Vector3.zero)
        {
            if (playTime == 0) return;
            if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Idle")) return;

            //2フレーム連続で止まっていた
            if (actionLog[playTime - 1] == Vector3.zero)
                animator.CrossFade("Idle", 0, 0);
            return;
        }

        /*移動している*/
        if (actionLog[playTime].y < 0.1f)
        {
            //下降中
            int temp = playTime + 5;
            if (temp >= actionLog.Count) return;
            //5フレーム先で着地しているか？
            if(actionLog[temp].y > -0.1f)
            {
                animator.CrossFade("TopToGround", 0.1f, 0);
            }
        }
        else if (actionLog[playTime].y > 0.1f)
        {
            //上昇中
            if(actionLog[playTime - 2].y < 0.1f) animator.CrossFade("JumpToTop", 0.1f, 0);
        }
        else
        {
            //地上()を歩いている
            animator.SetBool("IsMove", true);
        }

    }

    float ToRoundDown(float dValue, int iDigits)
    {
        float dCoef = (float)System.Math.Pow(10, iDigits);

        //Floorは指定した数値以下の最大の整数を取得する関数
        //Ceillingは指定した数値以上の最小の整数を取得する関数
        //参考：http://jeanne.wankuma.com/tips/csharp/math/rounddown.html

        return dValue > 0 ? (float)System.Math.Floor(dValue * dCoef) / dCoef :
                            (float)System.Math.Ceiling(dValue * dCoef) / dCoef;
    }
}
