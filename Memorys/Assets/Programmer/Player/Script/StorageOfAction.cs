using UnityEngine;
using System.Collections.Generic;

//todo:publicだらけなので要修正
//プレイヤーの行動を記憶しておくクラス
public class StorageOfAction
{
    public List<AnimationLoger> animationLog;
    GameObject player;
    Animator animator;
    Vector3 startPosition;
    Vector3 oldPosition;
    public List<Vector3> actionLog;

    private Quaternion StartRotation =Quaternion.identity;

    //記録再生の状態
    RecordState m_RecordState;
    //記録されているか？
    public bool IsRecorded { get { return actionLog.Count != 0; } }

    /*コンストラクタ*/
    public StorageOfAction(GameObject player, Animator animator)
    {
        this.player = player;
        this.animator = animator;

        //初期化
        startPosition = Vector3.zero;
        oldPosition = Vector3.zero;
        m_RecordState = RecordState.STAY;
        actionLog = new List<Vector3>();
        animationLog = new List<AnimationLoger>();
        PlayerController.I.AttackCallBack = () =>
        {
            //録画中でなければ必要ない
            if (RecordOfAction.I.m_RecordState != RecordState.RECORD) return;
            animationLog.Add(new AnimationLoger(actionLog.Count, AnimationLog.ATK));
            Debug.Log("AttackRecord");
        };
    }

    /*記録*/
    public void RecordStart()
    {
        m_RecordState = RecordState.RECORD;
        startPosition = player.transform.position;
        oldPosition = player.transform.position;
        StartRotation = Camera.main.transform.rotation;
        actionLog.Clear();
    }
    public void Recording()
    {
        //座標を記録する
        Vector3 movement = player.transform.position - oldPosition;
        //小数点5桁以下を切り捨てる
        movement.y = ToRoundDown(movement.y, 4);
        actionLog.Add(movement);
        oldPosition = player.transform.position;
    }
    public void StopRecord()
    {
        m_RecordState = RecordState.STAY;
    }

    /*再生*/
    public void StartAction()
    {
        m_RecordState = RecordState.PLAY;
        player.GetComponent<Rigidbody>().useGravity = false;
    }
    public void PlayingAction(int playTime)
    {
        Vector3 vec = Vector3.zero;
        vec += actionLog[playTime];
        //+1フレームして再生速度を倍にする(indexから出ていれば追加しない)
        if (actionLog.Count > playTime + 1)
        {
            vec += actionLog[playTime + 1];
        }
        if (vec == Vector3.zero) return;
        float rotationY =  Camera.main.transform.rotation.eulerAngles.y- StartRotation.eulerAngles.y;
        Quaternion rota = Quaternion.Euler(0,rotationY,0);
        Vector3 vec2 =  rota* vec;
        player.transform.position += vec2;
        player.transform.rotation =Quaternion.Euler(player.transform.rotation.x,Mathf.Atan2(vec2.x,vec2.z)*Mathf.Rad2Deg,player.transform.rotation.z);
        oldPosition = player.transform.position;
    }

    public void StopAction()
    {
        //actionLog.Clear();
        player.GetComponent<Rigidbody>().useGravity = true;
        m_RecordState = RecordState.STAY;
        for (int i = 0; i < animationLog.Count; i++)
        {
            animationLog[i].PlayEnd() ;
        }
    }

    //行動を解析しアニメーションを再生させる
    public void AnalysisBehaior(int playTime)
    {
        Debug.Log(actionLog[playTime].y);
        for (int i = 0; i < animationLog.Count; i++)
        {
            animationLog[i].PlayAnimation(playTime);
        }

        if (PlayerController.I.currentState == PlayerState.Attack) return;
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
        if (actionLog[playTime].y == 0)
        {
            //地上()を歩いている
            animator.SetBool("IsMove", true);
        }
        else if (actionLog[playTime].y < 0.01f)
        {
            //下降中
            player.GetComponent<PlayerController>().currentState = PlayerState.Fall;
            int temp = playTime + 5;
            if (temp >= actionLog.Count) return;
            //5フレーム先で着地しているか？
            if (actionLog[temp].y >= 0)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.TopToGround")) return;
                animator.CrossFade("TopToGround", 0.1f, 0);
            }
        }
        else
        {
            //上昇中
            if(actionLog[playTime - 2].y < 0.1f) animator.CrossFade("JumpToTop", 0.1f, 0);
        }

    }

    //指定の桁数以下を切り捨てる
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
