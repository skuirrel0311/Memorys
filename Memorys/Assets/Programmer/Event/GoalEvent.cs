﻿using System.Collections;
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
        RightDoor.transform.DOMoveX(0.0f, 2.0f);
        LeftDoor.transform.DOMoveX(0.0f, 2.0f);
        if (GameManager.I == null) return;
        GameManager.I.OnPossibleEscape += () =>
        {
            Debug.Log("OpenDoor");
            GameManager.I.IsPlayStop = true;
            PlayerController.I.currentState = PlayerState.Idle;
            CameraManager.I.CameraChange(0,3.0f,true,true,() => { GameManager.I.IsPlayStop = false; });
            RightDoor.transform.DOMoveX(-8.5f,3.0f);
            LeftDoor.transform.DOMoveX(8.5f,3.0f);
            UpperFloor.SetActive(true);
            UpperFloor.transform.DOMoveY(TargetPositionY, 3.0f);
        };
	}
}
