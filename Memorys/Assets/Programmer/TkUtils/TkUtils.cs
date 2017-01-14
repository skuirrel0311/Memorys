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

    public static float FloatLerp(float a, float b, float t)
    {
        t=Mathf.Clamp(t,0,1);

        return a + ((b - a) * t);
    }

    private static Vector2 GetAngle(Vector3 position, Vector3 target)
    {
        Vector2 angle = Vector2.zero;

        angle.x = Vector2.Angle(new Vector2(position.x, position.z), new Vector2(target.x, target.z));
        
        return angle;
    }

    public static float GetAngleY(Vector3 vec1, Vector3 vec2)
    {
        Vector3 temp = vec2 - vec1;
        float vecY = temp.y;
        //X方向だけのベクトルに変換
        temp = Vector3.right * temp.magnitude;
        temp.y = vecY;

        return Vector3.Angle(Vector3.right, temp) * 2.0f;
    }
}
