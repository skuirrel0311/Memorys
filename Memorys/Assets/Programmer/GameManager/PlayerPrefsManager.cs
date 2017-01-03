using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    public static PlayerPrefsManager I;


	// Use this for initialization
	void Start ()
    {
        I = this;
        DontDestroyOnLoad(gameObject);
	}

    public void FastSaveInt(string key,int value)
    {
        PlayerPrefs.SetInt(key,value);
    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public void SetClearTime(float time)
    {
        int num = PlayerPrefs.GetInt("StageNum");
        PlayerPrefs.SetFloat("Stage"+num,time);
    }

    public void SetCurrentClearTime(float time)
    {
        PlayerPrefs.SetFloat("CurrentTime",time);
        PlayerPrefs.Save();
    }

    public float GetCurrentClearTime()
    {
        return PlayerPrefs.GetFloat("CurrentTime");
    }

    public int GetStageNum()
    {
        return PlayerPrefs.GetInt("StageNum");
    }

    public float GetClearTime(int stagenum)
    {
        float n = PlayerPrefs.GetFloat("Stage" + stagenum);
        if(n<=0)
        {
            n = 6039;
        }
        return n;
    }
}
