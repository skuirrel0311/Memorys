using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvasManager : MonoBehaviour
{
    [SerializeField]
    GameObject GameOver;

	// Use this for initialization
	void Start ()
    {
        GameManager.I.m_GameEnd.OnGameOverCallBack += () => 
        {
            GameOver.SetActive(true);
        };
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
