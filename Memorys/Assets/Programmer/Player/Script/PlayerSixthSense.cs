using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class PlayerSixthSense : MonoBehaviour
{
    public float timer = 0.0f;
    [SerializeField]
    float startSenseTime = 6.0f;
    TotemPaul[] enemies;
    Light[] enemiesLight;

    Light directionalLight = null;

    //敵の視界を見るセンスがあるか？
    public bool hasSense = false;
    bool oldHasSense = false;

    bool IsWorkingCoroutine = false;
    Coroutine coroutine;

    void Start()
    {
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new TotemPaul[enemyArray.Length];
        enemiesLight = new Light[enemyArray.Length];
        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemies[i] = enemyArray[i].GetComponent<TotemPaul>();
            enemiesLight[i] = enemyArray[i].transform.GetChild(1).GetComponent<Light>();
        }
    }

    void Update()
    {
        //見つかっていなかったらtimerが増える
        if (WasSeen())
        {
            AkSoundEngine.SetState("BGM_change","Emergency");
        }
        else
        {
            AkSoundEngine.SetState("BGM_change", "Normal");
            
        }
    }

    //見つかったか？
    public bool WasSeen()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsWarning) return true;
        }

        //誰にも見つかっていなかった
        return false;
    }
}
