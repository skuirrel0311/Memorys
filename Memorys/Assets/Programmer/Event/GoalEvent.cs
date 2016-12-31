using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalEvent : MonoBehaviour {

    [SerializeField]
    GameObject RightDoor;
    [SerializeField]
    GameObject LeftDoor;

	// Use this for initialization
	void Start ()
    {

        GameManager.I.OnPossibleEscape += () =>
        {
            Debug.Log("OpenDoor");
            CameraManager.I.CameraChange(0,3.0f);
            RightDoor.transform.DOMoveX(-8.5f,3.0f);
            LeftDoor.transform.DOMoveX(8.5f,3.0f);
        };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
