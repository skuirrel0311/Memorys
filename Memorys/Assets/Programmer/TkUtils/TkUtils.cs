using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TkUtils
{
    public delegate void AbsChangeColor(Image image,Color color);
    

    public static IEnumerator Deray(float duration ,Action action)
    {
        yield return new WaitForSeconds(duration);
        action.Invoke();
    }

    public static IEnumerator Deray(float duration, AbsChangeColor action,Image image,Color color)
    {
        yield return new WaitForSeconds(duration);
        action(image, color);
    }

    public static IEnumerator DoColor(float duration,Image target,Color targetColor,bool isDouble=false)
    {
        float t = 0;
        Color startColor = target.color;
        while(true)
        {
            t += Time.deltaTime;
            if (isDouble)
            {
                float n = t / duration;
                target.color = Color.Lerp(startColor, targetColor, n*n);
            }
            else
            {
                target.color = Color.Lerp(startColor, targetColor, t / duration);
            }
            if (t >= duration) break;
            yield return null;
        }
    }
    public  static string PlasticTime(int time)
    {
        string s = string.Empty;
        int h = (int)((float)time / 60.0f);
        int m = (int)((float)time % 60.0f);

        if (h < 10)
        {
            s += "0";
        }

        s += h.ToString();

        s += ":";
        if (m < 10)
        {
            s += "0";
        }
        s += m.ToString();

        return s;
    }
}
