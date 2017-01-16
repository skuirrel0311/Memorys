using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HelpManager : MonoBehaviour
{
    [SerializeField]
    Image[] m_Pages;

    [SerializeField]
    RectTransform[] Tags;

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
    void Start()
    {
        PageCount = 0;

        //ページの有効化
        for (int i = 0; i < m_Pages.Length; i++)
        {
            if (i == m_pageCount)
            {
                m_Pages[i].gameObject.SetActive(true);
                Tags[i].anchoredPosition = new Vector2(100.0f, Tags[i].anchoredPosition.y);
            }
            else
            {
                m_Pages[i].gameObject.SetActive(false);
                Tags[i].anchoredPosition = new Vector2(0.0f, Tags[i].anchoredPosition.y);
            }
        }
        m_oldCount = PageCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickDown))
        {
            UtilsSound.SE_Select();
            PageCount = Mathf.Min(m_Pages.Length - 1, PageCount + 1);

        }
        else if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickUp))
        {
            UtilsSound.SE_Select();
            PageCount = Mathf.Max(0, PageCount - 1);

        }
    }

    void PageUpdate()
    {
        if (m_oldCount != PageCount)
        {
            //ページの有効化
            for (int i = 0; i < m_Pages.Length; i++)
            {
                if (i == m_pageCount)
                {
                    m_Pages[i].gameObject.SetActive(true);
                    StartCoroutine(TagMoveX(Tags[i],0.1f,100.0f));
                }
                else
                {
                    m_Pages[i].gameObject.SetActive(false);
                    StartCoroutine(TagMoveX(Tags[i], 0.1f, 0.0f));
                }
            }
            m_oldCount = PageCount;
        }
        else
        {

        }
    }

    IEnumerator TagMoveX(RectTransform target,float duration,float positionX)
    {
        float t=0.0f;

        Vector2 startPos = target.anchoredPosition;
        Vector2 targetpos = new Vector2(positionX,startPos.y);

        while(true)
        {
            t += Time.unscaledDeltaTime;
            target.anchoredPosition = Vector2.Lerp(startPos,targetpos,t/duration);
            if (t > duration) break;
            yield return null;
        }
        target.anchoredPosition = targetpos;
    }
}
