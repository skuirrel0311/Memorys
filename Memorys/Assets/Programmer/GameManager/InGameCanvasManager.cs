using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvasManager : MonoBehaviour
{
    [SerializeField]
    GameObject GameOver;

    [SerializeField]
    ParticleSystem GameClearParticles;

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

        GameManager.I.m_GameEnd.OnGameClearCallBack+= () =>
        {
            GameClearParticles.Play();
        };
        isPause = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (GameManager.I.IsPlayStop) return;
        if (GameEnd.isGameEnd) return;
		if(MyInputManager.GetButtonDown(MyInputManager.Button.Start))
        {
            Pause(!isPause);
        }
        else if(isPause&&MyInputManager.GetButtonDown(MyInputManager.Button.B))
        {
            Pause(!isPause);
        }
	}

    public void Pause(bool ispause)
    {
        isPause = ispause;
        if (isPause)
        {
            isPause = true;
            PauseObject.SetActive(true);
            Time.timeScale = 0.0f;
            GameManager.I.IsPlayStop = true;
        }
        else
        {
            isPause = false;
            PauseObject.GetComponent<Pause>().HelpDiasable();
            PauseObject.SetActive(false);
            Time.timeScale = 1.0f;
            GameManager.I.IsPlayStop = false;
        }
    }
}
