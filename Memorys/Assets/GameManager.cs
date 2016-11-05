using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    //ステージの崩壊間隔
    private const float c_IntervalSec = 30.0f;
    //一度に破壊されるオブジェクトの数
    private const int c_DestroyObjectNumber = 5;

    //プレイヤーの破壊目標
    private GameObject m_Target;
    //破壊目標の出現位置
    [SerializeField]
    private GameObject[] m_TargetPoints = null;
    //フィールド上の破壊可能オブジェクト
    private List<GameObject> m_FieldObjects;
    //ターゲットが破壊を宣言しているオブジェクト
    private List<GameObject> m_WillDestroyObjects;
    private float m_Interval=0.0f;

    private ParticleSystem m_SelectParticle;

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
    }

    private void Update()
    {
        m_Interval += Time.deltaTime;

        if(m_Interval > c_IntervalSec)
        {
            m_Interval = 0.0f;
            FieldObjectDestoy();
            SetWillDestroy();
            SetTargetRandom();
        }

        if (m_WillDestroyObjects.Count == c_DestroyObjectNumber)
        {
            for (int i = 0; i < c_DestroyObjectNumber; i++)
            {
                m_SelectParticle.transform.position = m_WillDestroyObjects[i].transform.position;
                m_SelectParticle.Emit(1);
            }
        }

    }

    //ターゲット（スイッチ）の場所をランダムで設置
    private void SetTargetRandom()
    {
        m_Target.transform.position = m_TargetPoints[Random.Range(0, m_TargetPoints.Length)].transform.position + Vector3.up;
    }

    //ターゲットが破壊しようとするオブジェクトを選択
    private void SetWillDestroy()
    {
        short[] ary = RandomShuffle();
        m_WillDestroyObjects.Clear();
        for (int i = 0; i < c_DestroyObjectNumber; i++)
        {
            m_WillDestroyObjects.Add(m_FieldObjects[ary[i]]);
        }
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(0,0,200,50),"残り"+(int)(c_IntervalSec - m_Interval));
    }


    public void DestroyCancel()
    {
        m_Interval = 0.0f;
        SetTargetRandom();
        SetWillDestroy();
    }

    public void FieldObjectDestoy()
    {
        for(int i=0; i < m_WillDestroyObjects.Count;i++)
        {
            m_FieldObjects.Remove(m_WillDestroyObjects[i]);
            m_WillDestroyObjects[i].AddComponent<Rigidbody>();
            Destroy(m_WillDestroyObjects[i],5.0f);
        }
    }

    private short[] RandomShuffle()
    {
        short[] ary = new short[m_FieldObjects.Count];
        short aryLength = (short)ary.Length;
        for(short i = 0; i< aryLength;i++)
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
