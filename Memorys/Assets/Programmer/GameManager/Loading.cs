﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SceneManager.LoadSceneAsync("stage"+PlayData.StageNum);	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
