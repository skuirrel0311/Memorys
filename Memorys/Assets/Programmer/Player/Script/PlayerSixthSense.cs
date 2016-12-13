using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSixthSense : MonoBehaviour
{
    public float timer = 0.0f;
    [SerializeField]
    float startSenseTime = 6.0f;
    TotemPaul[] enemies;
    Light[] enemiesLight;

    Light directionalLight = null;

    //敵の視界を見るセンスがあるか？
    bool hasSense = false;
    bool oldHasSense = false;

    bool IsWorkingCoroutine = false;
    Coroutine coroutine;

    bool wasSeen = false;

    public float sonarPower = 0.0f;
    [SerializeField]
    float maxSonarPower = 5.0f;

    [SerializeField]
    Text debugText = null;

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
        sonarPower = maxSonarPower;
    }

    void Update()
    {
        //見つかっていなかったらtimerが増える
        wasSeen = WasSeen();
        if (wasSeen)
        {
            timer -= Time.deltaTime * 5.0f;
        }
        else
        {
            timer += Time.deltaTime;
            if (hasSense) sonarPower += Time.deltaTime;
        }

        sonarPower = Mathf.Min(sonarPower, maxSonarPower);
        debugText.text = sonarPower.ToString("F2");

        timer = Mathf.Clamp(timer, 0.0f, startSenseTime);

        hasSense = (timer == startSenseTime);
        GetComponent<SoundWaveFinder>().IsUseable = (sonarPower == maxSonarPower);

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
            coroutine = StartCoroutine(SetLightSettings(Color.white, 5.0f, 0.3f, 0.0f));
        }

        if (hasSense == false && oldHasSense == true)
        {
            IsWorkingCoroutine = true;
            coroutine = StartCoroutine(SetLightSettings(Color.white, 1.0f, 1.0f, 1.0f));
        }
    }

    IEnumerator SetLightSettings(Color targetColor, float targetIntensity, float targetAmbientIntensity, float targetDirectionalIntensity)
    {
        float t = 0.0f;
        const float maxTime = 2.0f;

        float startIntensity = enemiesLight[0].intensity;
        Color startColor = enemiesLight[0].color;
        float startAmbientIntensity = RenderSettings.ambientIntensity;
        float startDirectionalIntensity = directionalLight.intensity;

        while (true)
        {
            t += Time.deltaTime;
            float progress = t / maxTime;

            //ライトの設定を敵に送る
            SetLight(
                FloatLerp(startIntensity, targetIntensity, progress)
                , Color.Lerp(startColor, targetColor, progress)
                );

            //アンビエントライトは強くする(0 -> 1)
            RenderSettings.ambientIntensity = FloatLerp(startAmbientIntensity, targetAmbientIntensity, progress);

            directionalLight.intensity = FloatLerp(startDirectionalIntensity, targetDirectionalIntensity, progress);
            if (t > maxTime) break;
            yield return null;
        }

        //誤差を修正する
        SetLight(targetIntensity, targetColor);
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
    bool WasSeen()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].IsWarning) return true;
        }

        //誰にも見つかっていなかった
        return false;
    }
}
