using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pause : MonoBehaviour
{
    [SerializeField]
    RectTransform[] Pages;

    Vector3[] Rotations= new Vector3[3];

    bool isRota;
    float Timer;
    void Awake()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            Rotations[i] = Pages[i].eulerAngles;
        }
    }

    void Initialize()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
             Pages[i].eulerAngles = Rotations[i];
        }
        isRota = true;
        Timer = 0.0f;
    }

    void OnEnable()
    {
        Initialize();

    }
	
	void Update ()
    {
        PageRotation();	
	}

    void PageRotation()
    {
        if (!isRota) return;
        Timer += Time.unscaledDeltaTime;
        for (int i = 0; i < Pages.Length; i++)
        {
            Pages[i].eulerAngles = Vector3.Lerp(Rotations[0], Vector3.forward * -5.0f * i,Timer*4.0f);
        }

        if (Timer > 0.25f)
        {
            isRota = false;
        }
    }
}
