using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[System.Serializable]
public class TimeLineEvent
{
    public bool FadeIn=false;
    public bool FadeOut=false;
    public int CameraIndex;
    public float Duration;
    public bool PlayOnAwake;
    public bool DoShake;
    public bool isNext;
    [SerializeField]
    public TimeLineEvent NextTimeLineEvent;

    public void Play(CameraManager cameraManager)
    {
        Action callBack = null;
        if (NextTimeLineEvent != null)
            callBack += () => { NextTimeLineEvent.Play(cameraManager); };
        cameraManager.CameraChange(CameraIndex, Duration, FadeIn,FadeOut,callBack);
        if (DoShake)
        {
            cameraManager.m_Cameras[CameraIndex].gameObject.transform.DOShakePosition(Duration, 0.1f, 30, 10, false, false);
        }
    }
}

public class CameraTimeLine : MonoBehaviour
{

    [SerializeField]
    TimeLineEvent[] m_TimeLineEvents;
    CameraManager m_CameraManager;

    // Use this for initialization
    void Start()
    {
        m_CameraManager = GetComponent<CameraManager>();
        for (int i = 0; i < m_TimeLineEvents.Length; i++)
        {
            if (m_TimeLineEvents[i].isNext)
            {
                m_TimeLineEvents[i-1].NextTimeLineEvent = m_TimeLineEvents[i];
            }           
        }

        for (int i = 0; i < m_TimeLineEvents.Length; i++)
        {
            if (m_TimeLineEvents[i].PlayOnAwake)
            {
                m_TimeLineEvents[i].Play(m_CameraManager);
            }
        }
    }
}
