using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CutMode
    {
        Transition,Cut
    }

    public static CameraManager I;
    /// <summary>
    /// メインカメラは含まれない
    /// </summary>
    [SerializeField]
    Camera[] m_Cameras;

    [SerializeField]
    float TransitionTime = 0.5f;

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
    }

    public void CameraChange(int index, float duration = 1.0f,CutMode cutMode = CutMode.Transition)
    {
        if (index >= m_Cameras.Length) return;
        if (cutMode == CutMode.Transition)
        {
            StartCoroutine(EnableTransitionCamera(m_Cameras[index], duration));
        }
        else if(cutMode==CutMode.Cut)
        {
            StartCoroutine(EnableCutCamera(m_Cameras[index], duration));
        }
    }

    public void CameraChange(string name, float duration = 1.0f, CutMode cutMode = CutMode.Transition)
    {
        for (int i = 0; i < m_Cameras.Length; i++)
        {
            if (!m_Cameras[i].gameObject.name.Equals(name)) continue;
            if (cutMode == CutMode.Transition)
            {
                StartCoroutine(EnableTransitionCamera(m_Cameras[i], duration));
            }
            else if (cutMode == CutMode.Cut)
            {
                StartCoroutine(EnableCutCamera(m_Cameras[i], duration));
            }
            break;
        }
    }

    IEnumerator EnableTransitionCamera(Camera camera, float wait)
    {
        Debug.Log("Callcut");
        TransitionManager.I.FadeOut(TransitionTime);
        yield return new WaitForSeconds(TransitionTime+0.1f);
        camera.enabled = true;
        TransitionManager.I.FadeIn(TransitionTime);
        yield return new WaitForSeconds(wait);
        TransitionManager.I.FadeOut(TransitionTime);
        yield return new WaitForSeconds(TransitionTime + 0.1f);
        camera.enabled = false;
        TransitionManager.I.FadeIn(TransitionTime);
    }

    IEnumerator EnableCutCamera(Camera camera, float wait)
    {
        camera.enabled = true;
        yield return new WaitForSeconds(wait);
        camera.enabled = false;
    }

    void OnDestroy()
    {
        if (I != null)
        {
            I = null;
        }
    }
}
