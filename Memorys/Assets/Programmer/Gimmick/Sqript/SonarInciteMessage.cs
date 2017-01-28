using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SonarInciteMessage : MonoBehaviour
{
    Image image;
    //前にスイッチを押してからの経過時間
    float intervalTime = 0.0f;
    
    [SerializeField]
    float limitTime = 60.0f;

    bool isViewMessage = false;

    List<Coroutine> coroutineList = new List<Coroutine>();

    SoundWaveFinder sonar;
    bool canUseSonar = true;

    Camera mainCamera;
    RectTransform canvasRect;

    bool isUseSonar = false;

    void Start()
    {
        image = GetComponent<Image>();
        sonar = GameObject.FindGameObjectWithTag("Player").GetComponent<SoundWaveFinder>();
        canvasRect = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>();
        mainCamera = Camera.main;

        GameManager.I.OnPushSwitch += () =>
        {
            StopViewMessage();
        };

        sonar.OnUseSonar += () =>
        {
            isUseSonar = true;
            StopViewMessage();
        };
    }

    void StopViewMessage()
    {
        intervalTime = 0.0f;

        if (isViewMessage)
        {
            isViewMessage = false;
            for (int i = 0; i < coroutineList.Count; i++)
            {
                StopCoroutine(coroutineList[i]);
            }

            coroutineList.Add(StartCoroutine(TkUtils.DoColor(1.0f, image, new Color(1.0f, 1.0f, 1.0f, 0.0f))));
        }
    }

    void Update()
    {
        if (isUseSonar) return;

        if (isViewMessage)
        {
            DrawMessage();
            return;
        }
        if (GameManager.I.IsPlayStop) return;
        if (sonar.power == 0) CanNotUseSonar();
        else canUseSonar = true;

        intervalTime += Time.deltaTime;

        if (intervalTime > limitTime)
        {
            if (!canUseSonar)
            {
                intervalTime = 0.0f;
                return;
            }

            if(coroutineList.Count != 0)
            {
                for (int i = 0; i < coroutineList.Count; i++)
                {
                    StopCoroutine(coroutineList[i]);
                }
            }
            
            coroutineList.Add(StartCoroutine(ViewInciteMessage()));
            isViewMessage = true;
        }
    }

    void DrawMessage()
    {
        image.rectTransform.anchoredPosition = GetPopUpPosition();
    }

    IEnumerator ViewInciteMessage()
    {
        Coroutine coroutine;
        WaitForSeconds wait =  new WaitForSeconds(0.5f);

        Color viewColor = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        Color invisibleColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        while (true)
        {
            coroutine = StartCoroutine(TkUtils.DoColor(0.8f, image, viewColor));
            coroutineList.Add(coroutine);
            yield return coroutine;

            yield return wait;

            coroutine = StartCoroutine(TkUtils.DoColor(0.8f, image, invisibleColor));
            coroutineList.Add(coroutine);
            yield return coroutine;
        }
    }

    void CanNotUseSonar()
    {
        canUseSonar = false;
        if(isViewMessage)
        {
            for (int i = 0; i < coroutineList.Count; i++)
            {
                StopCoroutine(coroutineList[i]);
            }
        }
    }

    Vector2 GetPopUpPosition()
    {
        Vector2 popUpPosition = mainCamera.WorldToViewportPoint(sonar.transform.position + Vector3.up * 2.2f);
        popUpPosition.x = (popUpPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
        popUpPosition.y = (popUpPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);
        return popUpPosition;
    }
}
