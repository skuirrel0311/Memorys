using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(MyInputManager.GetButtonDown(MyInputManager.Button.B))
        {
            UtilsSound.SE_Cancel();
            GetComponent<MySceneManager>().SceneLoad("Title");
        }
	}
}
