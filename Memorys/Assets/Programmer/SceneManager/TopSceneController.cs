using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class TopSceneController : MonoBehaviour
{
    MySceneManager m_sceneManager;


    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Push_A_Button();
        
    }
    private void Push_A_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        m_sceneManager.NextScene();
    }
}
