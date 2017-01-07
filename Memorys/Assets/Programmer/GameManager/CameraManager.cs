using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CutMode
    {
        Transition, Cut
    }

    public static CameraManager I;

    /// <summary>
    /// メインカメラは含まれない
    /// </summary>
    [SerializeField]
    public Camera[] m_Cameras;

    [SerializeField]
    float TransitionTime = 0.5f;

    [SerializeField]
    GameObject CinemaScopeCanvas;

    private Camera MainCamera;

    // Use this for initialization
    void Start()
    {
        I = this;
        MainCamera = Camera.main;

        for (int i = 0; i < m_Cameras.Length; i++)
        {
            m_Cameras[i].enabled = false;
        }

        CinemaScopeCanvas.SetActive(false);
    }

    public void CameraChange(int index, float duration = 1.0f, bool FadeIn = true, bool FadeOut = true, Action CallBack = null)
    {

        if (index >= m_Cameras.Length) return;
        StartCoroutine(EnableTransitionCamera(m_Cameras[index], duration, FadeIn, FadeOut, CallBack));
    }

    public void CameraChange(string name, float duration = 1.0f, bool FadeIn = true, bool FadeOut = true, Action CallBack = null)
    {
        for (int i = 0; i < m_Cameras.Length; i++)
        {
            if (!m_Cameras[i].gameObject.name.Equals(name)) continue;

            StartCoroutine(EnableTransitionCamera(m_Cameras[i], duration, FadeIn, FadeOut, CallBack));

            break;
        }
    }

    IEnumerator EnableTransitionCamera(Camera camera, float wait, bool FadeIn, bool FadeOut, Action action)
    {
        Debug.Log("Callcut");
        CinemaScopeCanvas.SetActive(true);
        //GameManager.I.IsPlayStop = true;
        if (FadeIn)
        {
            TransitionManager.I.FadeOut(TransitionTime);
            yield return new WaitForSeconds(TransitionTime + 0.1f);
            camera.enabled = true;
            TransitionManager.I.FadeIn(TransitionTime);
        }
        else
        {
            camera.enabled = true;
        }
        yield return new WaitForSeconds(wait);
        if (FadeOut)
        {
            TransitionManager.I.FadeOut(TransitionTime);
            yield return new WaitForSeconds(TransitionTime + 0.1f);
            camera.enabled = false;
            TransitionManager.I.FadeIn(TransitionTime);
        }
        else
        {
            camera.enabled = false;
        }
        CinemaScopeCanvas.SetActive(false);
        if (action != null)
            action();
        //GameManager.I.IsPlayStop = true;
    }


    void OnDestroy()
    {
        if (I != null)
        {
            I = null;
        }
    }
}
