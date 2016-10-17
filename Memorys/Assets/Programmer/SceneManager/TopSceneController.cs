using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class TopSceneController : MonoBehaviour
{
    MySceneManager m_sceneManager;
    [SerializeField]
    GameObject top;
    [SerializeField]
    GameObject select;

    enum TopSceneState
    {
        TOP, SELECT
    }

    enum SelectState
    {
        STAGE, HELP
    }

    TopSceneState m_TopSceneState;
    SelectState m_SelectState;
    [SerializeField]
    Image[] m_SelectChild;

    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        m_TopSceneState = TopSceneState.TOP;
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
        if (!Input.GetButtonDown("Fire1")) return;
        if (m_TopSceneState == TopSceneState.TOP)
        {
            top.transform.DOLocalMoveX(-1000.0f, 1.0f);
            select.transform.DOLocalMoveX(0.0f, 1.0f);
            m_TopSceneState++;
            m_SelectState = SelectState.STAGE;
        }
        else
        {
            if (m_SelectState == SelectState.STAGE)
            {
                m_sceneManager.NextScene();
            }
            else
            {

            }
        }
    }

    private void Push_B_Button()
    {
        if (!Input.GetButtonDown("Fire2")) return;
        if (m_TopSceneState == TopSceneState.SELECT)
        {
            top.transform.DOLocalMoveX(0.0f, 1.0f);
            select.transform.DOLocalMoveX(1000.0f, 1.0f);
            m_TopSceneState--;
        }
    }

    private void InputAxis()
    {
        if (m_TopSceneState != TopSceneState.SELECT) return;
        float v = Input.GetAxis("Vertical");
        if (v == 0)
        {
            return;
        }
        if(v>0)
        {
            m_SelectState = (SelectState)Mathf.Max(0,(float)m_SelectState-1);
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
        if(m_SelectState==SelectState.STAGE)
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
