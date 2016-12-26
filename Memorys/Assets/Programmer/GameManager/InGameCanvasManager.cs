using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvasManager : MonoBehaviour
{
    [SerializeField]
    GameObject GameOver;

    [SerializeField]
    GameObject PauseObject;
    bool isPause;

	// Use this for initialization
	void Start ()
    {
        GameManager.I.m_GameEnd.OnGameOverCallBack += () => 
        {
            GameOver.SetActive(true);
        };
        isPause = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(MyInputManager.GetButtonDown(MyInputManager.Button.Start))
        {
            if(!isPause)
            {
                isPause = true;
                PauseObject.SetActive(true);
                Time.timeScale = 0.0f;
                GameManager.I.IsPlayStop = true;
            }
            else
            {
                isPause = false;
                PauseObject.SetActive(false);
                Time.timeScale = 1.0f;
                GameManager.I.IsPlayStop = false;
            }
        }
	}
}
