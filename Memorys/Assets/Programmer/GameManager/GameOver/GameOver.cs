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
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
            StageSelect.color = Color.black;
        }
        else
        {
            Retry.color = Color.black;
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
