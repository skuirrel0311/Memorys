using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {
    AsyncOperation ao;
    // Use this for initialization
    void Start ()
    {
        ao =  SceneManager.LoadSceneAsync("stage"+PlayData.StageNum);
        ao.allowSceneActivation = false;
        StartCoroutine(TkUtils.Deray(2.0f,()=> { ao.allowSceneActivation = true; }));
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
