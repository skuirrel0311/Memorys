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

    Renderer[] targetRenderers;
    //もともと付いているマテリアル
    Material targetMat;
    //光らせるマテリアル
    Material strongMat;

    bool[] IsWorkingCoroutines;
    Coroutine[] coroutines;

    //ソナーが使用可能か？
    public bool IsUseable = true;

    //ソナーに反応があったか？
    bool IsFound = false;
    float longestWaitTime = 0.0f;

    void Start()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Target");
        targetRenderers = new Renderer[targetObjects.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            targetRenderers[i] = targetObjects[i].GetComponent<Renderer>();
        }

        targetMat = targetRenderers[0].material;
        strongMat = Resources.Load<Material>("Materials/TargetMat");
        waveParticle = Resources.Load<GameObject>("Particle/SoundWave");
        starParticle = Resources.Load<GameObject>("Particle/StarParticle");
        IsWorkingCoroutines = new bool[targetRenderers.Length];
        coroutines = new Coroutine[targetRenderers.Length];
    }

    void Update()
    {
        //消えたスイッチのコルーチンを止める
        for(int i = 0;i< coroutines.Length;i++)
        {
            if (targetRenderers[i] != null) continue;

            if (IsWorkingCoroutines[i]) StopCoroutine(coroutines[i]);
        }


        if (MyInputManager.GetButtonDown(MyInputManager.Button.Y) || Input.GetKeyDown(KeyCode.Y))
        {
            if (!IsUseable) return;

            IsFound = false;
            longestWaitTime = 0.0f;
            //パーティクルを生成
            Destroy(Instantiate(waveParticle, transform.position + (Vector3.up * 1.5f), Quaternion.identity), 1.5f);

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;
                SendSoundWave(targetRenderers[i], i);
            }

            GetComponent<PlayerSixthSense>().sonarPower = 0;

            if(IsFound)
            {
                //１つでも見つかったら
                StartCoroutine(DrawStarParticle(longestWaitTime));
            }
        }
    }

    //音波を発生させる
    void SendSoundWave(Renderer target, int index)
    {
        float distance = (target.transform.position - transform.position).magnitude;

        //範囲内にあったら光らせる
        if (distance < maxDistance)
        {
            IsFound = true;
            //音波が届くまでの時間を求める
            float waitTime = (distance / maxDistance) * 1.5f;

            longestWaitTime = Mathf.Max(waitTime, longestWaitTime);

            //光らせるコルーチンは起動しているかのチェックをする
            if (!IsWorkingCoroutines[index])
            {
                IsWorkingCoroutines[index] = true;
                coroutines[index] = StartCoroutine(SetMaterial(waitTime, target, index));
            }
            //帰ってくる音波はチェックしない
            float delay = 1.0f;
            StartCoroutine(DrawReturnWave(waitTime + delay, target.transform));
        }
    }

    //音波が届いたらマテリアルを差し替える
    IEnumerator SetMaterial(float waitTime, Renderer target, int index)
    {
        yield return new WaitForSeconds(waitTime);

        target.material = strongMat;

        yield return new WaitForSeconds(changingTime);

        target.material = targetMat;

        IsWorkingCoroutines[index] = false;
    }

    //帰ってくる音波だけを描画する
    IEnumerator DrawReturnWave(float waitTime, Transform target)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(Instantiate(waveParticle, target.position + Vector3.up, Quaternion.identity), 1.5f);
    }

    IEnumerator DrawStarParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(Instantiate(starParticle, transform.position + Vector3.up, Quaternion.identity), 1.5f);
    }


    bool IsWorkingAnyCoroutine()
    {
        for (int i = 0; i < IsWorkingCoroutines.Length; i++)
        {
            if (IsWorkingCoroutines[i]) return true;
        }

        return false;
    }

    bool IsAllBroke()
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null) return false;
        }

        return true;
    }
}
