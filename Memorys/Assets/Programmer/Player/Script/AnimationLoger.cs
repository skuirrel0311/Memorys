using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationLog
{
    DEF = 0, ATK = 1
}

public class AnimationLoger
{


    private int m_TimeCount=0;
    private AnimationLog m_log=0;
    private bool isPlayed;
    public   AnimationLoger(int timeCount,AnimationLog log)
    {
        m_TimeCount = timeCount;
        m_log = log;
        isPlayed = false;
    }

    public void PlayAnimation(int count)
    {
        if (count <= m_TimeCount) return;
        if (isPlayed == true) return;
            PlayerController.I.Attack();
        Debug.Log("PlayAttack");
        isPlayed = true;
    }

    public void PlayEnd()
    {
        isPlayed = false;
    }

    public void Clear()
    {
        m_TimeCount = 0;
        m_log = 0;
        isPlayed = false;
    }
}
