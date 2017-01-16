using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultManager : MonoBehaviour
{

    MySceneManager m_sceneManager;
    [SerializeField]
    GameObject select;

    [SerializeField]
    ClearRankData rankData;

    enum SelectState
    {
        SELECT, RETRY, NEXT, INDEX
    }

    SelectState m_SelectState;
    [SerializeField]
    Image[] m_SelectChild;

    [SerializeField]
    RectTransform m_CousorImage;

    [SerializeField]
    Text LevelText;

    [SerializeField]
    Text m_ClearTime;

    [SerializeField]
    Text m_BestTime;

    [SerializeField]
    Image m_NewRecord;

    [SerializeField]
    GameObject m_bestTimeObject;

    [SerializeField]
    GameObject m_ClearTimeObject;

    [SerializeField]
    Image[] m_RankSters = new Image[3];

    [SerializeField]
    Image[] m_BestSters = new Image[3];

    bool[] isResultEvent = new bool[3];

    bool isNext;
    bool IsLast;

    float m_time;
    int currntTime;
    int bestTime;
    bool isClearTime;
    // Use this for initialization
    void Start()
    {
        isNext = false;
        isClearTime = false;
        IsLast = false;

        m_time = 0.0f;
        m_sceneManager = GetComponent<MySceneManager>();
        m_SelectState = SelectState.SELECT;
        m_SelectChild = select.GetComponentsInChildren<Image>();
        if (PlayData.StageNum >= SelectManager.c_MaxStage)
        {
            IsLast = true;
            m_SelectChild[1].rectTransform.anchoredPosition =new Vector2(m_SelectChild[2].rectTransform.anchoredPosition.x, m_SelectChild[1].rectTransform.anchoredPosition.y);
            m_SelectChild[2].gameObject.SetActive(false);
        }
        SelectImageChange();


        LevelText.text = "ステージ " + PlayData.StageNum.ToString();
        currntTime = (int)PlayerPrefsManager.I.GetCurrentClearTime();
        bestTime = (int)PlayerPrefsManager.I.GetClearTime(PlayData.StageNum);

        if (bestTime > currntTime)
        {
            //new record
            bestTime = currntTime;
            m_NewRecord.gameObject.SetActive(true);
        }
        Debug.Log(currntTime.ToString());
        PlayerPrefsManager.I.SetClearTime((float)bestTime);
        PlayerPrefsManager.I.Save();
        m_BestTime.text = TkUtils.PlasticTime(bestTime);
    }

    public void RankStar(float currntTime)
    {
        float starTime = 1.5f;
        //クリアタイムのランク表示
        if (currntTime < rankData.Elements[PlayData.StageNum - 1].BestTime)
        {
            StarEnable(starTime + 0.6f, 2);
        }

        if (currntTime < rankData.Elements[PlayData.StageNum - 1].BetterTime)
        {
            StarEnable(starTime + 0.3f, 1);

        }
        StarEnable(starTime, 0);
    }

    public void StarEnable(float starTime, int StarIndex)
    {
        if (m_time > starTime && !isResultEvent[StarIndex])
        {
            isResultEvent[StarIndex] = true;
            StartCoroutine(TkUtils.DoColor(0.2f, m_RankSters[StarIndex], Color.white, true));
            m_RankSters[StarIndex].rectTransform.localScale = Vector3.one * 5;
            m_RankSters[StarIndex].rectTransform.DOScale(Vector3.one, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_time += Time.deltaTime;
        RankStar(currntTime);

        if (m_time > 0.5f)
        {
            m_bestTimeObject.SetActive(true);
            //ベストタイムのランク表示
            if (bestTime < rankData.Elements[PlayData.StageNum - 1].BestTime)
                m_BestSters[2].gameObject.SetActive(true);
            if (bestTime < rankData.Elements[PlayData.StageNum - 1].BetterTime)
                m_BestSters[1].gameObject.SetActive(true);
        }

        if (m_time > 1.0f && !isClearTime)
        {
            isClearTime = true;
            m_ClearTimeObject.SetActive(true);
            StartCoroutine(CurrentTimeUpdate());
        }

        if (m_time > 2.5f)
        {
            Push_A_Button();
            InputAxis();
        }
        else if (MyInputManager.GetButtonDown(MyInputManager.Button.A)
            || MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickLeft)
            || MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickRight))
        {
            m_time += 5.0f;
        }

    }

    IEnumerator CurrentTimeUpdate()
    {
        float t = 0.0f;
        Debug.Log(currntTime);
        while (true)
        {
            t += Time.deltaTime;
            m_ClearTime.text = TkUtils.PlasticTime((int)Mathf.Lerp(0, currntTime, t * 2.0f));
            if (t > 0.5f) break;
            yield return null;
        }
        Debug.Log(currntTime);
        m_ClearTime.text = TkUtils.PlasticTime(currntTime);
    }

    private void Push_A_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        if (isNext) return;
        isNext = true;
        UtilsSound.SE_Decision();
        if (m_SelectState == SelectState.SELECT)
        {
            SceneManager.LoadSceneAsync("StageSelect");
        }
        else if (m_SelectState == SelectState.RETRY)
        {
            SceneManager.LoadSceneAsync("Loading");
        }
        else if (m_SelectState == SelectState.NEXT)
        {
            PlayData.StageNum++;
            SceneManager.LoadSceneAsync("Loading");
        }

    }

    private void InputAxis()
    {
        if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickLeft))
        {
            UtilsSound.SE_Select();

            m_SelectState = (SelectState)Mathf.Max(0, (float)m_SelectState - 1);

            SelectImageChange();
        }
        else if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickRight))
        {
            UtilsSound.SE_Select();
            if (IsLast)
            {
                m_SelectState = (SelectState)Mathf.Min((float)SelectState.INDEX - 2, (float)m_SelectState + 1);
            }
            else
            {
                m_SelectState = (SelectState)Mathf.Min((float)SelectState.INDEX - 1, (float)m_SelectState + 1);
            }
            SelectImageChange();
        }
    }

    private void SelectImageChange()
    {
        for (int i = 0; i < m_SelectChild.Length; i++)
        {
            if (i == (int)m_SelectState)
            {
                m_SelectChild[i].color = Color.white;
                m_CousorImage.anchoredPosition = m_SelectChild[i].GetComponent<RectTransform>().anchoredPosition + Vector2.right * -50.0f;
            }
            else
            {
                m_SelectChild[i].color = Color.white * 0.5f;
            }
        }
    }
}
