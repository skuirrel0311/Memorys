using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//探知機
//音を発してスイッチを探す。スイッチが一定距離内に存在していたら発光する。
public class SoundWaveFinder : MonoBehaviour
{
    GameObject waveParticle;
    GameObject starParticle;

    //音が届く最大の距離
    [SerializeField]
    float maxDistance = 15.0f;
    //ターゲットを光らせる時間
    [SerializeField]
    float changingTime = 20.0f;

    Renderer targetRenderer;
    //もともと付いているマテリアル
    Material targetMat;
    //光らせるマテリアル
    Material strongMat;

    bool IsWorkingCoroutine = false;
    Coroutine coroutine;

    Timer timer;

    void Start()
    {
        targetRenderer = GameManager.I.m_Target.GetComponent<Renderer>();
        targetMat = targetRenderer.material;
        strongMat = Resources.Load<Material>("Materials/TargetMat");
        waveParticle = Resources.Load<GameObject>("Particle/SoundWave");
        starParticle = Resources.Load<GameObject>("Particle/StarParticle");

        timer = new Timer();
    }

    // Update is called once per frame
    void Update()
    {
        timer.Update();
        if (timer.IsLimitTime) timer.Stop(true);

        Vector3 myPosition = transform.position;
        Vector3 targetPosition = targetRenderer.transform.position;
        myPosition.y = 0.0f;
        targetPosition.y = 0.0f;

        float distance = Vector3.Distance(myPosition, targetPosition);

        if (MyInputManager.GetButtonDown(MyInputManager.Button.Y) || Input.GetKeyDown(KeyCode.Y))
        {
            if (timer.IsWorking) return;
            SendSoundWave();
        }
    }

    //音波を発生させる
    void SendSoundWave()
    {
        //パーティクルを生成
        Destroy(Instantiate(waveParticle, transform.position + (Vector3.up * 1.5f), Quaternion.identity), 1.5f);

        float distance = (targetRenderer.transform.position - transform.position).magnitude;

        //範囲内にあったら光らせる
        if (distance < maxDistance)
        {
            //音波が届くまでの時間を求める
            float waitTime = (distance / maxDistance) * 1.5f;

            if (timer.IsWorking) return;

            //光らせるコルーチンは起動しているかのチェックをする
            if (!IsWorkingCoroutine)
            {
                IsWorkingCoroutine = true;
                coroutine = StartCoroutine(SetMaterial(waitTime, distance));
            }

            timer.TimerStart(waitTime + 1.5f);
            //帰ってくる音波はチェックしない
            StartCoroutine(DrawReturnWave(waitTime, distance));
        }
        else
        {
            timer.TimerStart(1.5f);
        }
    }

    //音波が届いたらマテリアルを差し替える
    IEnumerator SetMaterial(float waitTime, float distance)
    {
        yield return new WaitForSeconds(waitTime);

        targetRenderer.material = strongMat;

        yield return new WaitForSeconds(changingTime);

        targetRenderer.material = targetMat;
        IsWorkingCoroutine = false;
    }

    //帰ってくる音波だけを描画する
    IEnumerator DrawReturnWave(float waitTime, float distance)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(Instantiate(waveParticle, targetRenderer.transform.position + Vector3.up, Quaternion.identity), 1.5f);
        Destroy(Instantiate(starParticle, transform.position + Vector3.up, Quaternion.identity), 1.5f);
    }
}
