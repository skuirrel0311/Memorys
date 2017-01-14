using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class TopSceneController : MonoBehaviour
{
    [SerializeField]
    float m_WaitTime = 60;

    MySceneManager m_sceneManager;

    [SerializeField]
    MovieOnUI m_MovieOnUI;

    [SerializeField]
    Image TitleRogo;

    [SerializeField]
    GameObject MenuCanvas;

    [SerializeField]
    GameObject TitleMenu;

    float m_Timer;


    static bool isAwake=false;

    bool isNext;
    bool isMenu;
    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        isNext = false;
        isMenu = false;
        if (!isAwake)
        {
            m_MovieOnUI.Play();
            isAwake = true;
        }
        else
        {
            m_MovieOnUI.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Push_A_Button();
        m_Timer += Time.deltaTime;
        if(m_Timer>=m_WaitTime&&!m_MovieOnUI.IsPlaying)
        {
            m_MovieOnUI.gameObject.SetActive(true);
            m_MovieOnUI.Play();
        }  
    }
    private void Push_A_Button()
    {
        if (m_MovieOnUI.IsPlaying)
        {
            //入力に使いたいボタンが押されていなければ戻る
            if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)
                &&!MyInputManager.GetButtonDown(MyInputManager.Button.Start))return;
            m_MovieOnUI.Stop();
            m_MovieOnUI.gameObject.SetActive(false);
            m_Timer = 0.0f;
            return;
        }
        if (MyInputManager.GetButtonDown(MyInputManager.Button.A)||
            MyInputManager.GetButtonDown(MyInputManager.Button.Start)||
            MyInputManager.GetButtonDown(MyInputManager.Button.B))
        {
            if (!isMenu)
            {
                isMenu = true;
                MenuCanvas.SetActive(true);
                TitleMenu.SetActive(false);
                return;
            }
        }
    }

    public void NextStageSelect()
    {
        if (isNext) return;
        isNext = true;
        Camera.main.gameObject.transform.DOMoveZ(-65.0f, 2.0f);
        GetComponent<DoorAnimation>().OpenDoor();
        StartCoroutine(TkUtils.DoColor(1.0f, TitleRogo, Color.clear));
        StartCoroutine(TkUtils.Deray(2.0f, () => { m_sceneManager.SceneLoad("StageSelect"); }));
    }
}
