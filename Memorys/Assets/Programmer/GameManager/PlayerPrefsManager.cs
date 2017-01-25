using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    public static PlayerPrefsManager I;


	// Use this for initialization
	void Awake ()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        int num = PlayData.StageNum;
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
        return PlayData.StageNum;
    }

    public float GetClearTime(int stagenum)
    {
        float n = PlayerPrefs.GetFloat("Stage" + stagenum);      
        if (n<=0)
        {
            n = 5999;
        }
        return n;
    }
}
