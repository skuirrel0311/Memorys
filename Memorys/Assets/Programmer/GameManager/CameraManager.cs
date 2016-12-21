using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager I;
    /// <summary>
    /// メインカメラは含まれない
    /// </summary>
    [SerializeField]
    Camera[] m_Cameras;

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

    public void CameraChange(int index, float duration = 1.0f)
    {
        if (index >= m_Cameras.Length) return;
        StartCoroutine(EnableCutCamera(m_Cameras[index], duration));
    }

    public void CameraChange(string name, float duration = 1.0f)
    {
        for (int i = 0; i < m_Cameras.Length; i++)
        {
            if (!m_Cameras[i].gameObject.name.Equals(name)) continue;
            StartCoroutine(EnableCutCamera(m_Cameras[i], duration));
            break;
        }
    }

    IEnumerator EnableCutCamera(Camera camera, float wait)
    {
        MainCamera.enabled = false;
        camera.enabled = true;
        yield return new WaitForSeconds(wait);
        camera.enabled = false;
        MainCamera.enabled = true;
    }

    void OnDestroy()
    {
        if (I != null)
        {
            I = null;
        }
    }
}
