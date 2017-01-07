using System.Collections;
using System;
using UnityEngine;

public class TkUtils
{
    public static IEnumerator Deray(float duration ,Action action)
    {
        yield return new WaitForSeconds(duration);
        action.Invoke();
    }
}
