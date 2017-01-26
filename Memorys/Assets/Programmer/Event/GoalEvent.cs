using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GoalEvent : MonoBehaviour {

    [SerializeField]
    GameObject RightDoor;
    [SerializeField]
    GameObject LeftDoor;

    [SerializeField]
    GameObject UpperFloor;

    [SerializeField]
    float TargetPositionY = -0.64f;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(TkUtils.Vibration(1.7f, 0.5f,()=> 
        {
            StartCoroutine(TkUtils.Vibration(0.7f, 1.0f));
        }));
        RightDoor.transform.DOMoveX(0.0f, 2.0f);
        LeftDoor.transform.DOMoveX(0.0f, 2.0f);
        if (GameManager.I == null) return;
        GameManager.I.OnPossibleEscape += () =>
        {
            Debug.Log("OpenDoor");
            AkSoundEngine.PostEvent("Open_Door",gameObject);
            StartCoroutine(TkUtils.Vibration(2.0f, 0.5f));
            GameManager.I.IsPlayStop = true;
            PlayerController.I.currentState = PlayerState.Idle;
            CameraManager.I.CameraChange(0,2.5f,true,true,() => { GameManager.I.IsPlayStop = false; });
            RightDoor.transform.DOMoveX(-8.5f,2.0f);
            LeftDoor.transform.DOMoveX(8.5f,2.0f);
            UpperFloor.SetActive(true);
            UpperFloor.transform.DOMoveY(TargetPositionY, 2.0f);
        };
	}
}
