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

        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    void Update()
    {
        //見つかっていなかったらtimerが増える
        if (WasSeen())
        {
            timer -= Time.deltaTime * 5.0f;
        }
        else
        {
            timer += Time.deltaTime;

        }

        timer = Mathf.Clamp(timer, 0.0f, startSenseTime);

        hasSense = (timer == startSenseTime);


        UpdateLight();

        oldHasSense = hasSense;

    }

    void UpdateLight()
    {
        if (IsWorkingCoroutine)
        {
            if ((hasSense == true && oldHasSense == false) || (hasSense == false && oldHasSense == true))
            {
                //コルーチンが走っている間に変更したい場合
                StopCoroutine(coroutine);
                IsWorkingCoroutine = false;
            }
            else
            {
                return;
            }
        }

        if (hasSense == true && oldHasSense == false)
        {
            IsWorkingCoroutine = true;
            coroutine = StartCoroutine(SetLightSettings(5.0f,Color.white, 5.0f, 0.5f, 0.5f));
        }

        if (hasSense == false && oldHasSense == true)
        {
            IsWorkingCoroutine = true;
            coroutine = StartCoroutine(SetLightSettings(1.0f,Color.white, 1.0f, 1.0f, 1.0f));
        }
    }

    IEnumerator SetLightSettings(float time,Color targetColor, float targetIntensity, float targetAmbientIntensity, float targetDirectionalIntensity)
    {
        float t = 0.0f;
        //const float maxTime = 2.0f;

        float startIntensity = enemiesLight[0].intensity;
        Color startColor = enemiesLight[0].color;
        float startAmbientIntensity = RenderSettings.ambientIntensity;
        float startDirectionalIntensity = directionalLight.intensity;
        ChromaticAberrationModel ca = Camera.main.GetComponent<PostProcessingBehaviour>().profile.chromaticAberration;
        ChromaticAberrationModel.Settings cas = ca.settings;
        while (true)
        {
            t += Time.deltaTime;
            float progress = t / time;
            RenderSettings.ambientIntensity = FloatLerp(startDirectionalIntensity, targetDirectionalIntensity, progress);
            directionalLight.intensity = FloatLerp(startDirectionalIntensity, targetDirectionalIntensity, progress);
            cas.intensity = (1 - FloatLerp(startDirectionalIntensity, targetDirectionalIntensity, progress)) * 0.5f;
            ca.settings = cas;
            if (t > time) break;
            yield return null;
        }

        //誤差を修正する
        //SetLight(targetIntensity, targetColor);
        RenderSettings.ambientIntensity = targetAmbientIntensity;
        directionalLight.intensity = targetDirectionalIntensity;
        IsWorkingCoroutine = false;
    }

    float FloatLerp(float a, float b, float t)
    {
        return a - ((a - b) * t);
    }

    void SetLight(float intensity, Color color)
    {
        for (int i = 0; i < enemiesLight.Length; i++)
        {
            enemiesLight[i].intensity = intensity;
            //enemiesLight[i].color = color;
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
