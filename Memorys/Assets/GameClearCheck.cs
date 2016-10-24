using UnityEngine;
using System.Collections;

public class GameClearCheck : MonoBehaviour {

    bool isClear;
    void Start()
    {
        isClear = false;
    }

    void Update()
    {
        if (!isClear) return;
        
        if(MyInputManager.GetButtonDown(MyInputManager.Button.A))
        {
            Time.timeScale = 1.0f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }

     void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            isClear = true;
            Time.timeScale = 0.0f;
        }

    }
}
