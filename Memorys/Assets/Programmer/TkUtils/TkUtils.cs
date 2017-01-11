using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TkUtils
{
    public static IEnumerator Deray(float duration ,Action action)
    {
        yield return new WaitForSeconds(duration);
        action.Invoke();
    }

    public static IEnumerator DoColor(float duration,Image target,Color targetColor)
    {
        float t = 0;
        Color startColor = target.color;
        while(true)
        {
            t += Time.deltaTime;
            target.color = Color.Lerp(startColor,targetColor,t/duration);
            if (t >= duration) break;
            yield return null;
        }
    }
}
