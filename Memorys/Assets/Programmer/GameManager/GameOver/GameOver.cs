using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    Image Retry;

    [SerializeField]
    Image StageSelect;

    bool isLeft = true;

    bool isWait;
    float timer = 0.0f;

    void OnEnable()
    {
        timer = 0.0f;
        isWait = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            isWait = false;
        }
        if (isWait) return;

        if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickLeft))
        {
            isLeft = true;
        }
        else if (MyInputManager.IsJustStickDown(MyInputManager.StickDirection.LeftStickRight))
        {
            isLeft = false;
        }
        
        if(isLeft)
        {
            Retry.color = Color.white;
            StageSelect.color = Color.white*0.5f;
        }
        else
        {
            Retry.color = Color.white * 0.5f;
            StageSelect.color = Color.white;
        }

        if(MyInputManager.GetButtonDown(MyInputManager.Button.A))
        {
            if (isLeft)
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            else
                SceneManager.LoadSceneAsync("StageSelect");
        }
    }
}
