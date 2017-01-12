using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour {

    MySceneManager m_sceneManager;
    [SerializeField]
    GameObject select;
    enum SelectState
    {
        SELECT,RETRY,NEXT, INDEX
    }

    SelectState m_SelectState;
    [SerializeField]
    Image[] m_SelectChild;

    [SerializeField]
    RectTransform m_CousorImage;

    [SerializeField]
    Text m_ClearTime;

    [SerializeField]
    Text m_BestTime;

    [SerializeField]
    Image m_NewRecord;

    [SerializeField]
    Image[] m_RankSters=new Image[3];

    [SerializeField]
    Image[] m_BestSters = new Image[3];

    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        m_SelectState = SelectState.SELECT;
        m_SelectChild = select.GetComponentsInChildren<Image>();
        SelectImageChange();

        int currntTime = (int)PlayerPrefsManager.I.GetCurrentClearTime();
        int bestTime = (int)PlayerPrefsManager.I.GetClearTime(PlayData.StageNum);
        if(bestTime>currntTime)
        {
            //new record
            bestTime = currntTime;
            m_NewRecord.gameObject.SetActive(true);
        }
        PlayerPrefsManager.I.SetClearTime((float)bestTime);
        PlayerPrefsManager.I.Save();
        m_ClearTime.text = currntTime.ToString();
        m_BestTime.text = bestTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Push_A_Button();
        InputAxis();

    }

    private void Push_A_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        if (m_SelectState == SelectState.SELECT)
        {
            m_sceneManager.NextScene();
        }
        if(m_SelectState == SelectState.RETRY)
        {
            SceneManager.LoadSceneAsync("Loading");
        }
        else
        {
            PlayData.StageNum++;
            SceneManager.LoadSceneAsync("Loading");
        }

    }

    private void InputAxis()
    {
        if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickLeft))
        {
            m_SelectState = (SelectState)Mathf.Max(0, (float)m_SelectState - 1);
            SelectImageChange();
        }
        else if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickRight))
        {
            m_SelectState = (SelectState)Mathf.Min((float)SelectState.INDEX - 1, (float)m_SelectState + 1);
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
