using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        UnEnabledSwitchImage();
        GameManager.I.m_GameEnd.OnDestroyCancelCallBack += () =>
        {
            SwitchUpdate();
        };
	}

    void UnEnabledSwitchImage()
    {
        for(int i=0;i<SwitchImages.Length;i++)
        {
            if(i>=GameManager.I.m_TargetPoints.Length)
            {
                SwitchImages[i].gameObject.SetActive(false);
            }
        }
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
        Image switchImage = SwitchImages[PushCount];
        switchImage.sprite = m_enabledImage;
        switchImage.color = Color.white*0.8f;
        switchImage.rectTransform.localScale = Vector3.one * 2.0f;
        switchImage.rectTransform.DOScale(Vector3.one,0.7f);
        PushCount++;
	}
}
