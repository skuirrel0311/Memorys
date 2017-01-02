using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using BehaviorDesigner.Runtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public GameEnd m_GameEnd;

    public bool IsPlayStop { get; set; }

    [SerializeField]
    private GameObject GameClearLogo = null;

    //地形の変化が起きる時間の間隔
    [SerializeField]
    private float transitionInterval = 0.1f;

    //プレイヤーの破壊目標
    private GameObject m_Target;
    private GameObject m_TargetPoint;
    //破壊目標の出現位置
    [SerializeField]
    private GameObject[] m_TargetPoints = null;
    //フィールド上の破壊可能オブジェクト
    public List<GameObject> m_FieldObjects;
    //ターゲットが破壊を宣言しているオブジェクト
    private List<GameObject> m_WillDestroyObjects;

    public BehaviorTree[] enemies;
    private AtScreenEdgeMessage[] directionMessages;
    
    private GameObject floorObj = null;

    //ゴールまでの床
    [SerializeField]
    private GameObject toGoalFloor = null;

    [SerializeField]
    public bool OneByOne = true;

    [SerializeField]
    public bool IsFlat = false;

    public delegate void VoidEvent();
    public VoidEvent OnPossibleEscape;

    public VoidEvent OnPushSwitch;

    private void Awake()
    {

        m_GameEnd = new GameEnd();
        m_GameEnd.Initialize();

        m_GameEnd.OnGameClearCallBack = () =>
        {
            //GameClearLogo.GetComponent<RectTransform>().DOMoveY(200.0f, 0.1f, true).SetLoops(0, LoopType.Yoyo);
            SceneManager.LoadSceneAsync("Result");
        };

        //ターゲットのオブジェクトを取得してポジションをセットする
        if (OneByOne)
            m_Target = GameObject.Instantiate(Resources.Load("Prefabs/Switch") as GameObject) as GameObject;
        else
            m_Target = Resources.Load<GameObject>("Prefabs/Switch");

        if (m_TargetPoints != null)
        {
            if(OneByOne)
                SetTargetRandom();
            else
                SetTarget();

            GameEnd.c_MaxDestroyCalcel = m_TargetPoints.Length;
        }

        I = this;
    }

    private void Start()
    {
        // I = this;
        IsPlayStop = true;
        StartCoroutine(GameStartWait(4));
        //エフェクトのデータを取得
        GameObject go = Instantiate(Resources.Load("Particle/Select") as GameObject);
        floorObj = Resources.Load("Prefabs/FloorObject") as GameObject;


        //破壊可能オブジェクトの取得
        m_FieldObjects = new List<GameObject>();
        m_FieldObjects.AddRange(GameObject.FindGameObjectsWithTag("FieldObject"));

        m_WillDestroyObjects = new List<GameObject>();
        // SetWillDestroy();
        NotificationSystem.I.Indication("スイッチを" + GameEnd.c_MaxDestroyCalcel.ToString() + "回押し、脱出せよ！");
        if (!IsFlat) StartCoroutine("SetObjTransition");

        SetEventPushSwitch();
        InitializeEnemy();
    }


    IEnumerator GameStartWait(float wait)
    {
       yield  return new WaitForSeconds(wait);
        IsPlayStop = false;
    }
    private void SetEventPushSwitch()
    {
        OnPushSwitch += () =>
        {
            m_GameEnd.DestroyCancel();
            GenerateEnemy();
            //クリア条件を満たしている
            if (m_GameEnd.m_destoryCancelCount >= GameEnd.c_MaxDestroyCalcel)
            {
                toGoalFloor.SetActive(true);
                NotificationSystem.I.Indication("脱出可能になった！");
                if (OnPossibleEscape != null)
                    OnPossibleEscape();

                if (OneByOne) Destroy(m_Target);
                return;
            }

            if (OneByOne) SetTargetRandom();
        };
    }

    private void InitializeEnemy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Image messagePrefab = (Resources.Load("Prefabs/DirectionMessage") as GameObject).GetComponent<Image>();

        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new BehaviorTree[tempArray.Length];
        directionMessages = new AtScreenEdgeMessage[tempArray.Length];
        for (int i = 0; i < tempArray.Length; i++)
        {
            enemies[i] = tempArray[i].GetComponent<BehaviorTree>();
            directionMessages[i] = player.AddComponent<AtScreenEdgeMessage>() as AtScreenEdgeMessage;
            directionMessages[i].targetTransform = enemies[i].transform;
            directionMessages[i].messagePrefab = messagePrefab;

            //中心のトーテムポール
            if (enemies[i].gameObject.name == "TotemPaul")
            {
                enemies[i].GetComponent<TotemPaul>().QuickStartUp();
            }
        }
    }

    private void OnDestroy()
    {
        StopCoroutine("SetObjTransition");
        I = null;
    }

    private void Update()
    {
        m_GameEnd.Update();
        UpdateDirectionMessage();
        //m_Target.transform.position = m_TargetPoint.transform.position;
    }

    private void UpdateDirectionMessage()
    {
        for (int i = 0; i < directionMessages.Length; i++)
        {
            TotemPaul t = enemies[i].GetComponent<TotemPaul>();
            if (!enemies[i].enabled)
            {
                directionMessages[i].enabled = false;
                continue;
            }
            directionMessages[i].enabled = true;
            directionMessages[i].IsViewMessage = t.Alertness > 0.5f;
            directionMessages[i].messagePrefab.fillAmount = (t.Alertness / 3.0f);
        }
    }

    private void ObjectEmission(GameObject obj, Color color)
    {
        if (obj == null) return;
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
            m_TargetPoint = m_TargetPoints[Random.Range(0, m_TargetPoints.Length)];
            m_Target.transform.position = m_TargetPoint.transform.position + Vector3.up;
            //地形の変動と一緒に動くように親を設定する
            m_Target.transform.parent = m_TargetPoint.transform.parent;

            //同じ場所ではなかったらbreak
            if (NowPos.x != m_Target.transform.position.x)
            {
                break;
            }
        }
    }

    private void SetTarget()
    {
        for (int i = 0; i < m_TargetPoints.Length; i++)
        {
            GameObject g = Instantiate(m_Target, m_TargetPoints[i].transform);
            g.transform.localPosition = Vector3.zero;
        }
    }

    //ターゲットが破壊しようとするオブジェクトを選択
    private void SetWillDestroy()
    {
        //short[] ary = RandomShuffle();
        m_WillDestroyObjects.Clear();

        float distance = 50.0f - m_GameEnd.StageDestroyCount * 10.0f;
        distance *= distance;
        int wallSeceletCount = 0;
        for (int i = 0; i < m_FieldObjects.Count; i++)
        {
            if (wallSeceletCount <= 5 && m_FieldObjects[i].GetComponent<MeshFilter>().sharedMesh.name == "wall")
            {
                wallSeceletCount++;
                m_WillDestroyObjects.Add(m_FieldObjects[i]);
                ObjectEmission(m_FieldObjects[i], Color.red);
            }

            if (Vector3.SqrMagnitude(m_FieldObjects[i].transform.position) > distance)
            {
                m_WillDestroyObjects.Add(m_FieldObjects[i]);
                ObjectEmission(m_FieldObjects[i], Color.red);
            }
        }
    }

    public void DestroyCancel()
    {
        //SetTargetRandom();
        //SetWillDestroy();
        m_GameEnd.DestroyCancel();
    }

    public void FieldObjectDestoy()
    {
        for (int i = 0; i < m_WillDestroyObjects.Count; i++)
        {
            if (m_WillDestroyObjects[i] != null)
                StartCoroutine("ObjectDestroy", m_WillDestroyObjects[i]);
        }
        m_GameEnd.StageDestroy();
    }

    IEnumerator ObjectDestroy(GameObject go)
    {
        float rand = Random.Range(0.1f, 1.0f);
        yield return new WaitForSeconds(rand);
        m_FieldObjects.Remove(go);
        go.AddComponent<Rigidbody>();
        go.GetComponent<BoxCollider>().enabled = false;
        Destroy(go, 5.0f);
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

    private IEnumerator SetObjTransition()
    {
        var wait = new WaitForSeconds(transitionInterval);
        while (true)
        {
            m_FieldObjects[Random.Range(0, m_FieldObjects.Count)].GetComponent<FloorTransition>().FloorTrans();
            yield return wait;
        }
    }

    public void PushSwitch()
    {
        if(OnPushSwitch != null)
        {
            OnPushSwitch();
        }
    }

    private void GenerateEnemy()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            //中心のトーテムポール
            if (enemies[i].gameObject.name == "TotemPaul (" + m_GameEnd.m_destoryCancelCount.ToString() + ")")
            {
                enemies[i].GetComponent<TotemPaul>().StartUp();
            }
        }
    }
}
