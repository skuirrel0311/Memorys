using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public GameEnd m_GameEnd;
    //ステージの崩壊間隔
    private const float c_IntervalSec = 30.0f;
    //一度に破壊されるオブジェクトの数
    private const int c_DestroyObjectNumber = 80;

    [SerializeField]
    private GameObject GameClearLogo;

    //プレイヤーの破壊目標
    private GameObject m_Target;
    //破壊目標の出現位置
    [SerializeField]
    private GameObject[] m_TargetPoints = null;
    //フィールド上の破壊可能オブジェクト
    private List<GameObject> m_FieldObjects;
    //ターゲットが破壊を宣言しているオブジェクト
    private List<GameObject> m_WillDestroyObjects;
    private float m_Interval = 0.0f;

    private ParticleSystem m_SelectParticle;
    [SerializeField]
    LimitTime limitTime;

    private void Awake()
    {
        m_GameEnd = new GameEnd();
        m_GameEnd.Initialize();

        m_GameEnd.OnGameClearCallBack = () =>
        {
            GameClearLogo.GetComponent<RectTransform>().DOMoveY(200.0f, 0.1f, true).SetLoops(0, LoopType.Yoyo);

        };

        m_GameEnd.OnGameOverCallBack = () =>
        {
            GameClearLogo.GetComponent<UnityEngine.UI.Image>().color = Color.black;
              GameClearLogo.GetComponent<RectTransform>().DOMoveY(200.0f, 0.1f, true).SetLoops(0, LoopType.Yoyo);
            Debug.Log("GameeOverCallBack",this);
        };
    }

    private void Start()
    {
        I = this;

        //ターゲットのオブジェクトを取得してポジションをセットする
        m_Target = GameObject.Instantiate(Resources.Load("Prefabs/Target") as GameObject) as GameObject;
        if (m_TargetPoints != null)
        {
            SetTargetRandom();
        }
        //エフェクトのデータを取得
        GameObject go = Instantiate(Resources.Load("Particle/Select") as GameObject);
        m_SelectParticle = go.GetComponent<ParticleSystem>();

        //破壊可能オブジェクトの取得
        m_FieldObjects = new List<GameObject>();
        m_FieldObjects.AddRange(GameObject.FindGameObjectsWithTag("FieldObject"));

        m_WillDestroyObjects = new List<GameObject>();
        m_Interval = 0.0f;
        SetWillDestroy();
        NotificationSystem.I.Indication("ターゲットを５回破壊し、崩壊を止めろ！");
    }

    private void Update()
    {
        m_Interval += Time.deltaTime;
        limitTime.DrawTime((int)((c_IntervalSec - m_Interval) / 0.01f), 4);
        if (m_Interval > c_IntervalSec)
        {
            m_Interval = 0.0f;
            FieldObjectDestoy();
            SetWillDestroy();
            SetTargetRandom();
        }

        m_GameEnd.Update();
    }

    private void ObjectEmission(GameObject obj,Color color)
    {
        //オブジェクトを光らせる
        Renderer r = obj.GetComponent<Renderer>();
        r.material.EnableKeyword("_EMISSION");
        r.material.SetColor("_EmissionColor", color); 
    }

    //ターゲット（スイッチ）の場所をランダムで設置
    private void SetTargetRandom()
    {
        Vector3 NowPos = m_Target.transform.position;
        while (true)
        {
            m_Target.transform.position = m_TargetPoints[Random.Range(0, m_TargetPoints.Length)].transform.position + Vector3.up;
            if (!NowPos.Equals(m_Target.transform.position))
            {
                break;
            }
        }
    }

    //ターゲットが破壊しようとするオブジェクトを選択
    private void SetWillDestroy()
    {
        short[] ary = RandomShuffle();
        m_WillDestroyObjects.Clear();
        for (int i = 0; i < c_DestroyObjectNumber; i++)
        {
            m_WillDestroyObjects.Add(m_FieldObjects[ary[i]]);
            ObjectEmission(m_FieldObjects[ary[i]],Color.red);
        }
    }

    public void DestroyCancel()
    {
        m_Interval = 0.0f;
        for(int i = 0;i < m_WillDestroyObjects.Count;i++)
        {
            ObjectEmission(m_WillDestroyObjects[i],Color.black);
        }
        SetTargetRandom();
        SetWillDestroy();
        m_GameEnd.DestroyCancel();
    }

    public void FieldObjectDestoy()
    {
        for (int i = 0; i < m_WillDestroyObjects.Count; i++)
        {
            m_FieldObjects.Remove(m_WillDestroyObjects[i]);
            m_WillDestroyObjects[i].AddComponent<Rigidbody>();
            Destroy(m_WillDestroyObjects[i], 5.0f);
        }
    }

    private short[] RandomShuffle()
    {
        short[] ary = new short[m_FieldObjects.Count];
        short aryLength = (short)ary.Length;
        for (short i = 0; i < aryLength; i++)
        {
            ary[i] = i;
        }

        //Fisher-Yatesアルゴリズムでシャッフルする
        System.Random rng = new System.Random();
        short n = aryLength;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            short tmp = ary[k];
            ary[k] = ary[n];
            ary[n] = tmp;
        }

        return ary;
    }

}
