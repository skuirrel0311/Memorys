using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchManage : MonoBehaviour {

    [SerializeField]
    Sprite m_enabledImage;

    [SerializeField]
    Image[] SwitchImages;

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
	
	// Update is called once per frame
	void SwitchUpdate ()
    {
        SwitchImages[PushCount].sprite = m_enabledImage;
        PushCount++;
	}
}
