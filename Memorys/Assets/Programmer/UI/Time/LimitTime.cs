using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LimitTime : MonoBehaviour
{
    [SerializeField]
    Image[] digits;
    Sprite[] numberSprites;
    float m_timer;

    void Start()
    {
        //0～9の10個
        numberSprites = new Sprite[10];

        digits = transform.FindChild("TimeSprite").GetComponentsInChildren<Image>();
        for (int i = 0; i < 10; i++)
        {
           numberSprites[i] = Resources.Load<Sprite>("Time/time_" + i.ToString());
        }
        DrawTime(1234, 4);

        GameManager.I.m_GameEnd.OnGameClearCallBack += () =>
         {
             PlayerPrefsManager.I.SetCurrentClearTime(m_timer);
         };
    }

    void FixedUpdate()
    {
        if (GameManager.I.IsPlayStop)return;
        m_timer += Time.deltaTime;
       
        DrawTime(((int)m_timer/60*100)+((int)m_timer%60),4);
    }

    //表示したい時間をint型にまとめて渡してね　例：12:34 → DrawTime(1234,4);
    public void DrawTime(int time,int digit)
    {
        if (time < 0) return;
        //各桁の数字　例1234 → digitNumber[0] = 1 , digitNumber[1] = 2,…
        int[] digitNumber = new int[digit];

        for(int i = digit - 1;i >= 0;i--)
        {
            int temp = digit - 1 - i;
            digitNumber[temp] = time / (int)Mathf.Pow(10, i);
            time = time % (int)Mathf.Pow(10, i);
        }

        for(int i = 0;i < digits.Length;i++)
        {
            digits[i].sprite = numberSprites[digitNumber[i]];
        }
    }
}
