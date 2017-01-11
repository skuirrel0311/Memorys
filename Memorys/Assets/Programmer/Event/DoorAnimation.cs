using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorAnimation : MonoBehaviour {

    [SerializeField]
    GameObject RightDoor;
    [SerializeField]
    GameObject LeftDoor;

    public void OpenDoor()
    {
        RightDoor.transform.DOMoveX(-8.5f, 3.0f);
        LeftDoor.transform.DOMoveX(8.5f, 3.0f);
    }
}
