using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    [SerializeField]
    Image[] m_Pages;

    [SerializeField]
    RectTransform []Tags;

    int m_pageCount;
    int PageCount
    {
        get
        {
            return m_pageCount;
        }
        set
        {
            m_pageCount = value;
            PageUpdate();
        }
    }
    int m_oldCount;

	// Use this for initialization
	void Start ()
    {
        PageCount = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickDown))
        {
            PageCount = Mathf.Min(m_Pages.Length-1,PageCount+1);

        }
        else if(MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickUp))
        {
            PageCount = Mathf.Max(0, PageCount - 1);

        }
	}

    void PageUpdate()
    {
        if(m_oldCount!=PageCount)
        {
            //ページの有効化
            for(int i= 0;i< m_Pages.Length;i++)
            {
                if(i==m_pageCount)
                {
                    m_Pages[i].gameObject.SetActive(true);
                    Tags[i].anchoredPosition = new Vector2(100.0f,Tags[i].anchoredPosition.y);
                }
                else
                {
                    m_Pages[i].gameObject.SetActive(false);
                    Tags[i].anchoredPosition = new Vector2(0.0f, Tags[i].anchoredPosition.y);
                }
            }
            m_oldCount = PageCount;
        }
        else
        {

        }
    }
}
