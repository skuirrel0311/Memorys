using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//探知機
//音を発してスイッチを探す。スイッチが一定距離内に存在していたら発光する。
public class SoundWaveFinder : MonoBehaviour
{
    //エフェクト
    GameObject waveParticle;
    GameObject starParticle;

    //音が届く最大の距離
    [SerializeField]
    float maxDistance = 30.0f;
    //ターゲットを光らせる時間
    [SerializeField]
    float changingTime = 10.0f;
    
    Renderer[] targetRenderers;
    //もともと付いているマテリアル
    Material targetMat;
    //光らせるマテリアル
    Material strongMat;

    bool[] IsWorkingCoroutines;
    Coroutine[] coroutines;
    
    [SerializeField]
    float power = 0.0f;
    [SerializeField]
    float maxPower = 5.0f;
    PlayerSixthSense sense;

    //ソナーに反応があったか？
    bool IsFound = false;
    float longestWaitTime = 0.0f;
    
    public Timer workingTimer;

    [SerializeField]
    Text debugText = null;

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
        sense = GetComponent<PlayerSixthSense>();
        power = maxPower;

        workingTimer = new Timer();
        workingTimer.Stop(true);
    }

    void Update()
    {
        //消えたスイッチのコルーチンを止める
        for(int i = 0;i< coroutines.Length;i++)
        {
            if (targetRenderers[i] != null) continue;
            
            if (IsWorkingCoroutines[i]) StopCoroutine(coroutines[i]);
        }

        workingTimer.Update();
        if(workingTimer.IsLimitTime)
        {
            workingTimer.Stop(true);
        }
        //if (sense.hasSense) power += Time.deltaTime;
        //power = Mathf.Min(power, maxPower);

        if (debugText != null)
        {
            if (power > 0)
                debugText.text = "あと " + power.ToString() + "回";
            else
                debugText.text = "使用不可";
        }

        if (MyInputManager.GetButtonDown(MyInputManager.Button.Y) || Input.GetKeyDown(KeyCode.Y))
        {
            if (power <= 0) return;
            AkSoundEngine.PostEvent("Player_Search",gameObject);
            IsFound = false;
            longestWaitTime = 0.0f;
            power -= 1.0f;
            //パーティクルを生成
            Destroy(Instantiate(waveParticle, transform.position + (Vector3.up * 1.5f), Quaternion.identity), 1.5f);

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;
                SendSoundWave(targetRenderers[i], i);
            }


            workingTimer.TimerStart(longestWaitTime);
            if (IsFound)
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
            AkSoundEngine.PostEvent("Player_Discovery",target.gameObject);
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
    
    //マテリアルを切り替える処理を停止させる
    public void StopSetMaterial(int index = 0)
    {
        if (!IsWorkingCoroutines[index]) return;
        IsWorkingCoroutines[index] = false;
        StopCoroutine(coroutines[index]);
        targetRenderers[index].material = targetMat;
    }
}
