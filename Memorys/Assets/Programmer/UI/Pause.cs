using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Pause : MonoBehaviour
{
    enum ButtonSelect
    {
        CONTINUE,RETRY,TITLE,HELP
    }

    [SerializeField]
    RectTransform[] Pages;
    [SerializeField]
    InGameCanvasManager m_InGameManager;
    [SerializeField]
    GameObject HelpCanvas;

    Vector3[] Rotations= new Vector3[3];

    bool isRota;
    float Timer;

    private ButtonSelect m_buttonSelect;
    private ButtonSelect m_ButtonSelect
    {
        get
        {
            return m_buttonSelect;
        }
        set
        {
            m_buttonSelect = value;
            isCalc = true;
        }
       
    }
    bool isCalc;
    bool isHelp;

    //ButtonSelectの順にアサイン
    [SerializeField]
    Image[] m_Images;

    void Awake()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            Rotations[i] = Pages[i].eulerAngles;
        }
    }

    void Initialize()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
             Pages[i].eulerAngles = Rotations[i];
        }
        isRota = true;
        isHelp = false;
        Timer = 0.0f;
        m_ButtonSelect = ButtonSelect.CONTINUE;
    }

    void OnEnable()
    {
        Initialize();

    }
	
	void Update ()
    {
        PageRotation();
        if (isHelp)
        {
            if (MyInputManager.GetButtonDown(MyInputManager.Button.B))
            {
                HelpCanvas.SetActive(false);
                isHelp = false;
            }
            return;
        }
        ButtonUpdate();
        PushButton();
	}

    void PageRotation()
    {
        if (!isRota) return;
        Timer += Time.unscaledDeltaTime;
        for (int i = 0; i < Pages.Length; i++)
        {
            Pages[i].eulerAngles = Vector3.Lerp(Rotations[0], Vector3.forward * -5.0f * i,Timer*4.0f);
        }

        if (Timer > 0.25f)
        {
            isRota = false;
        }
    }

    void ButtonUpdate()
    {
        SelectAxis();
        if (!isCalc) return;
        int bs = (int)m_ButtonSelect;
        for (int i = 0; i < m_Images.Length; i++)
        {
            if (bs == i)
            {
                m_Images[i].color = Color.white;
            }
            else
            {
                m_Images[i].color = Color.white * 0.5f;
            }
        }
    }

    void SelectAxis()
    {
        if(MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickDown))
        {
            if((int)m_ButtonSelect+1>=m_Images.Length)
            {
                return;
            }
            else
            {
                //todo:音
                m_ButtonSelect++;
            }
        }
        else if(MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickUp))
        {
            if ((int)m_ButtonSelect-1 < 0)
            {
                return;
            }
            else
            {
                m_ButtonSelect--;
            }
        }
    }

    void PushButton()
    {


        if (!MyInputManager.GetButtonDown(MyInputManager.Button.A)) return;

        switch (m_ButtonSelect)
        {
            case ButtonSelect.CONTINUE:
                {
                    m_InGameManager.Pause(false);
                    break;
                }
            case ButtonSelect.HELP:
                {
                    isHelp = true;
                    HelpCanvas.SetActive(true);
                    break;
                }
            case ButtonSelect.TITLE:
                {
                    m_InGameManager.Pause(false);
                    SceneManager.LoadSceneAsync("Title");
                    break;
                }
            case ButtonSelect.RETRY:
                {
                    m_InGameManager.Pause(false);
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                    break;
                }
        }
    }
}
