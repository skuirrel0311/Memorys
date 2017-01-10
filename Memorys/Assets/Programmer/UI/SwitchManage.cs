using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchManage : MonoBehaviour {

    [SerializeField]
    Sprite m_enabledImage;

    [SerializeField]
    Image[] SwitchImages;

    [SerializeField]
    ParticleSystem particle;

    [SerializeField]
    Camera m_prticleCamera;

    int PushCount;

	// Use this for initialization
	void Start ()
    {
        PushCount = 0;
        GameManager.I.m_GameEnd.OnDestroyCancelCallBack += () =>
        {
            SwitchUpdate();
        };
	}
	void PositionUpdate()
    {
        //Vector3 w = RectTransformUtility.WorldToScreenPoint(m_prticleCamera, SwitchImages[PushCount].);
        float width = Screen.width;
        float height = Screen.height;
        Vector3 w = SwitchImages[PushCount].rectTransform.parent.GetComponent<RectTransform>().anchoredPosition3D+ SwitchImages[PushCount].rectTransform.anchoredPosition3D;
        w.x = (w.x+ (0.5f*1920.0f)) / 1920.0f* width;
        w.y = w.y / 1080.0f * height+ (height * 0.5f);
        particle.gameObject.transform.position = m_prticleCamera.ScreenToWorldPoint(w)+Vector3.forward;
    }
	// Update is called once per frame
	void SwitchUpdate ()
    {

        PositionUpdate();
        particle.Play();
        SwitchImages[PushCount].sprite = m_enabledImage;
        PushCount++;
	}
}
