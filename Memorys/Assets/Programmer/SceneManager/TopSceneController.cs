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
    float m_Timer;


    

    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        m_MovieOnUI.Play();
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
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;


        m_sceneManager.NextScene();
    }
}
