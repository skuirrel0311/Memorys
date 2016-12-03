using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ターゲットの方向を知らせるUI
public class AtScreenEdgeMessage : PopUpMessage
{
    //ターゲットの位置
    public Transform targetTransform;

    //スクリーンの中心座標
    protected Vector2 screenOrigin;

    public override void Start()
    {
        base.Start();
        
        screenOrigin = new Vector2(canvasRect.sizeDelta.x * 0.5f, canvasRect.sizeDelta.y * 0.5f);
    }

    public override void DrawMessage()
    {
        if (messagePrefab == null) return;
        messagePrefab.gameObject.SetActive(IsViewMessage);

        if (!IsViewMessage) return;
        if (targetTransform == null) return;

        messagePrefab.rectTransform.anchoredPosition = GetEdgePosition(targetTransform.position) - screenOrigin;
    }

    //指定された方向のエッジ(画面端)の座標を返す。
    protected Vector2 GetEdgePosition(Vector3 targetPosition)
    {
        Vector2 vec = ToEdgeVector(targetPosition).normalized;
        Vector2 temp = screenOrigin;
        while (true)
        {
            temp += vec;
            if (IsOutScreen(temp)) return temp -= vec * 100;
        }
    }

    Vector2 ToEdgeVector(Vector3 targetPosition)
    {
        Quaternion cameraQuate = Quaternion.Euler(0, -Camera.main.transform.eulerAngles.y, 0);
        //カメラに写すのだからカメラ(カメラの所持者)からターゲットへのベクトルを用いる
        Vector3 temp = cameraQuate * (targetPosition - transform.position);
        return new Vector2(temp.x, temp.z);
    }

    bool IsOutScreen(Vector2 pos)
    {
        if (pos.x < 0) return true;
        if (pos.y < 0) return true;
        if (pos.x > canvasRect.sizeDelta.x) return true;
        if (pos.y > canvasRect.sizeDelta.y) return true;

        return false;
    }
}
