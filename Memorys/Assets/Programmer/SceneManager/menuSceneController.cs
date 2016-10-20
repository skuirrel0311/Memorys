using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class menuSceneController : MonoBehaviour
{

    MySceneManager m_sceneManager;
    [SerializeField]
    GameObject select;
    enum SelectState
    {
        STAGE, HELP
    }

    SelectState m_SelectState;
    [SerializeField]
    Image[] m_SelectChild;

    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        m_SelectState = SelectState.STAGE;
        m_SelectChild = select.GetComponentsInChildren<Image>();
        SelectImageChange();
    }

    // Update is called once per frame
    void Update()
    {
        Push_A_Button();
        Push_B_Button();
        InputAxis();

    }
    private void Push_A_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        if (m_SelectState == SelectState.STAGE)
        {
            m_sceneManager.NextScene();
        }
        else
        {

        }

    }

    private void Push_B_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.B)) return;
    }

    private void InputAxis()
    {
        float v = MyInputManager.GetAxis(MyInputManager.Axis.LeftStick).y;
        if (v == 0)
        {
            return;
        }
        if (v > 0)
        {
            m_SelectState = (SelectState)Mathf.Max(0, (float)m_SelectState - 1);
            SelectImageChange();
        }
        else
        {
            m_SelectState = (SelectState)Mathf.Min((float)SelectState.HELP, (float)m_SelectState + 1);
            SelectImageChange();
        }
    }

    private void SelectImageChange()
    {
        if (m_SelectState == SelectState.STAGE)
        {
            m_SelectChild[0].color = Color.red;
            m_SelectChild[1].color = Color.white;
        }
        else
        {
            m_SelectChild[0].color = Color.white;
            m_SelectChild[1].color = Color.red;
        }
    }
}
