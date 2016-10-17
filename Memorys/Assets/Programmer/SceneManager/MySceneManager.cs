using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MySceneManager : MonoBehaviour {

    [SerializeField]
    string NextSceneName;

    public delegate void UpdateCallBack();
    public UpdateCallBack OnUpdateCallBack;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(OnUpdateCallBack!=null)
        {
            OnUpdateCallBack();
        }
	}

    public void NextScene()
    {
        SceneManager.LoadScene(NextSceneName);
    }
}
