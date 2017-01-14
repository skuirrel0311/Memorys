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
        STAGE, HELP,EXIT,INDEX
    }

    SelectState m_SelectState;
    [SerializeField]
    Image[] m_SelectChild;

    [SerializeField]
    Image SelectBack;

    [SerializeField]
    RectTransform m_CousorImage;

    [SerializeField]
    TopSceneController m_topSceneController;

    Image[] MenuImages;

    // Use this for initialization
    void Start()
    {
        m_sceneManager = GetComponent<MySceneManager>();
        m_SelectState = SelectState.STAGE;
        m_SelectChild = select.GetComponentsInChildren<Image>();
        SelectImageChange();
        MenuImages = gameObject.transform.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Push_A_Button();
        //Push_B_Button();
        InputAxis();

    }
    private void Push_A_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;
        if (m_SelectState == SelectState.STAGE)
        {
            for (int i = 0; i < MenuImages.Length; i++)
            {
                StartCoroutine(TkUtils.DoColor(1.0f, MenuImages[i],Color.clear));
            }
            m_topSceneController.NextStageSelect();
        }
        else if (m_SelectState == SelectState.HELP)
        {
            m_sceneManager.SceneLoad("Help");
        }
        else if (m_SelectState== SelectState.EXIT)
        {
            Application.Quit();
        }

    }

    private void Push_B_Button()
    {
        if (!MyInputManager.GetButtonDown(MyInputManager.Button.B)) return;
        m_sceneManager.SceneLoad("Title");
    }

    private void InputAxis()
    {
        if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickUp))
        {
            m_SelectState = (SelectState)Mathf.Max(0, (float)m_SelectState - 1);
            SelectImageChange();
        }
        else if(MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickDown))
        {
            m_SelectState = (SelectState)Mathf.Min((float)SelectState.INDEX-1, (float)m_SelectState + 1);
            SelectImageChange();
        }
    }

    private void SelectImageChange()
    {
        for(int i= 0;i< m_SelectChild.Length;i++)
        {
            if(i==(int)m_SelectState)
            {
                m_SelectChild[i].color = Color.white;
                m_CousorImage.anchoredPosition = m_SelectChild[i].GetComponent<RectTransform>().anchoredPosition+Vector2.right*-50.0f;
                SelectBack.rectTransform.sizeDelta = new Vector2(m_CousorImage.sizeDelta.x+m_SelectChild[i].rectTransform.sizeDelta.x+50.0f ,SelectBack.rectTransform.sizeDelta.y);
                SelectBack.rectTransform.anchoredPosition = m_CousorImage.anchoredPosition - ((Vector2.right * m_CousorImage.sizeDelta.x)+(Vector2.right * -50));
            }
            else
            {
                m_SelectChild[i].color = Color.white * 0.8f;
            }
        }
    }
}
