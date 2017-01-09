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
        particle.gameObject.transform.parent  = SwitchImages[PushCount].transform;
        particle.gameObject.transform.localPosition = new Vector3(-39.0f, -50.0f, 0.0f);
        particle.Play();
        SwitchImages[PushCount].sprite = m_enabledImage;
        PushCount++;
	}
}
